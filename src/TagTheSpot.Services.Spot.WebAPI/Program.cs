using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Extensions;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Integrations;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using TagTheSpot.Services.Shared.API.DependencyInjection;
using TagTheSpot.Services.Shared.API.Factories;
using TagTheSpot.Services.Shared.API.Middleware;
using TagTheSpot.Services.Shared.Application.Extensions;
using TagTheSpot.Services.Shared.Auth.DependencyInjection;
using TagTheSpot.Services.Shared.Auth.Options;
using TagTheSpot.Services.Shared.Infrastructure.Options;
using TagTheSpot.Services.Shared.Messaging.Submissions;
using TagTheSpot.Services.Shared.Messaging.Users;
using TagTheSpot.Services.Spot.Application.Abstractions.AI;
using TagTheSpot.Services.Spot.Application.Abstractions.Geo;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.Abstractions.Storage;
using TagTheSpot.Services.Spot.Application.Consumers;
using TagTheSpot.Services.Spot.Application.Mappers;
using TagTheSpot.Services.Spot.Application.Options;
using TagTheSpot.Services.Spot.Application.Services;
using TagTheSpot.Services.Spot.Application.Validators;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Domain.Spots;
using TagTheSpot.Services.Spot.Domain.Submissions;
using TagTheSpot.Services.Spot.Domain.Users;
using TagTheSpot.Services.Spot.Infrastructure.Extensions;
using TagTheSpot.Services.Spot.Infrastructure.Options;
using TagTheSpot.Services.Spot.Infrastructure.Persistence;
using TagTheSpot.Services.Spot.Infrastructure.Persistence.Repositories;
using TagTheSpot.Services.Spot.Infrastructure.Services;

namespace TagTheSpot.Services.Spot.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var swaggerXmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var swaggerXmlFilePath = Path.Combine(AppContext.BaseDirectory, swaggerXmlFileName);

            builder.Services.ConfigureSwaggerGen(swaggerXmlFilePath);
            builder.Services.AddJsonMultipartFormDataSupport(JsonSerializerChoice.Newtonsoft);

            builder.Services.AddHttpClient<ISubmissionModerationService, GroqSubmissionModerationService>((sp, client) =>
            {
                var groqSettings = sp.GetRequiredService<IOptions<GroqApiSettings>>().Value;

                client.DefaultRequestHeaders.Add(
                    HeaderNames.Accept,
                    MediaTypeNames.Application.Json);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    JwtBearerDefaults.AuthenticationScheme,
                    groqSettings.ApiKey);
            });

            builder.Services.AddDbContext<ApplicationDbContext>(
                (serviceProvider, options) =>
                {
                    var dbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>().Value;

                    options.UseNpgsql(
                        dbSettings.ConnectionString, options =>
                        {
                            options.UseNetTopologySuite();
                        });
                });

            builder.Services.ConfigureValidatableOnStartOptions<RabbitMqSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<JwtSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<DbSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<MessagingSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<DataSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<AzureBlobStorageSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<AzureContentSafetySettings>();
            builder.Services.ConfigureValidatableOnStartOptions<LocationValidationSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<GroqApiSettings>();
            builder.Services.ConfigureValidatableOnStartOptions<SubmissionModerationSettings>();

            builder.Services.ConfigureAuthentication();

            builder.Services.AddSingleton<ICityRepository, CityRepository>();
            builder.Services.AddScoped<ICityService, CityService>();

            builder.Services.AddSingleton<IGeoValidationService, UAGeoValidationService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<ISpotRepository, SpotRepository>();
            builder.Services.AddScoped<ISpotService, SpotService>();

            builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<ISubmissionService, SubmissionService>();

            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddSingleton<IContentSafetyService, AzureContentSafetyService>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<AddSpotRequestValidator>();

            builder.Services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<UserCreatedEventConsumer>();
                cfg.AddConsumer<SubmissionRejectedEventConsumer>();
                cfg.AddConsumer<SubmissionApprovedEventConsumer>();

                cfg.UsingRabbitMq((context, config) =>
                {
                    var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                    var messagingSettings = context.GetRequiredService<IOptions<MessagingSettings>>().Value;

                    config.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    config.ReceiveEndpoint(messagingSettings.QueueName, e =>
                    {
                        e.Bind<UserCreatedEvent>();
                        e.ConfigureConsumer<UserCreatedEventConsumer>(context);

                        e.Bind<SubmissionRejectedEvent>();
                        e.ConfigureConsumer<SubmissionRejectedEventConsumer>(context);

                        e.Bind<SubmissionApprovedEvent>();
                        e.ConfigureConsumer<SubmissionApprovedEventConsumer>(context);
                    });
                });
            });

            builder.Services.AddSingleton((serviceProvider) =>
            {
                var blobStorageSettings = serviceProvider.GetRequiredService<IOptions<AzureBlobStorageSettings>>().Value;

                var blobServiceClient = new BlobServiceClient(blobStorageSettings.ConnectionString);

                var containerClient = blobServiceClient.GetBlobContainerClient(blobStorageSettings.ContainerName);

                containerClient.CreateIfNotExists();

                var properties = containerClient.GetProperties();

                if (properties.Value.PublicAccess != PublicAccessType.Blob)
                {
                    containerClient.SetAccessPolicy(PublicAccessType.Blob);
                }

                return containerClient;
            });

            builder.Services.AddSingleton<IBlobService, AzureBlobStorageService>();

            builder.Services.AddMappersFromAssembly(typeof(AddSpotRequestToSpotMapper).Assembly);

            builder.Services.AddSingleton<ProblemDetailsFactory>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDevelopmentCorsPolicy();

            var app = builder.Build();

            app.UseExceptionHandlingMiddleware();

            if (app.Environment.IsDevelopment())
            {
                app.UseCors(CorsExtensions.DevelopmentPolicyName);
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.ApplyMigrations();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
