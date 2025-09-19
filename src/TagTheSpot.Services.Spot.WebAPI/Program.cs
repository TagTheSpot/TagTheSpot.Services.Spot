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
using TagTheSpot.Services.Shared.Messaging.Events.Submissions;
using TagTheSpot.Services.Shared.Messaging.Events.Users;
using TagTheSpot.Services.Shared.Messaging.Options;
using TagTheSpot.Services.Spot.Application.Abstractions.AI;
using TagTheSpot.Services.Spot.Application.Abstractions.Geo;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.Abstractions.Storage;
using TagTheSpot.Services.Spot.Application.Consumers;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Application.Extensions;
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
using TagTheSpot.Services.Spot.Infrastructure.Persistence.Options;
using TagTheSpot.Services.Spot.Infrastructure.Persistence.Repositories;
using TagTheSpot.Services.Spot.Infrastructure.Services;
using TagTheSpot.Services.Spot.WebAPI.Extensions;
using TagTheSpot.Services.Spot.WebAPI.Factories;
using TagTheSpot.Services.Spot.WebAPI.Middleware;

namespace TagTheSpot.Services.Spot.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

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

            builder.Services.AddJsonMultipartFormDataSupport(JsonSerializerChoice.Newtonsoft);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureSwaggerGen();

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

            builder.Services.AddOptions<DbSettings>()
                .BindConfiguration(DbSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<DataSettings>()
                .BindConfiguration(DataSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<RabbitMqSettings>()
                .BindConfiguration(RabbitMqSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<MessagingSettings>()
                .BindConfiguration(MessagingSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<AzureBlobStorageSettings>()
                .BindConfiguration(AzureBlobStorageSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<AzureContentSafetySettings>()
                .BindConfiguration(AzureContentSafetySettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration(JwtSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<LocationValidationSettings>()
                .BindConfiguration(LocationValidationSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<GroqApiSettings>()
                .BindConfiguration(GroqApiSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<SubmissionModerationSettings>()
                .BindConfiguration(SubmissionModerationSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

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

            builder.Services.AddMapper<AddSpotRequest, Domain.Spots.Spot, AddSpotRequestToSpotMapper>();
            builder.Services.AddMapper<Submission, SubmissionResponse, SubmissionToSubmissionResponseMapper>();
            builder.Services.AddMapper<Submission, Domain.Spots.Spot, SubmissionToSpotMapper>();
            builder.Services.AddMapper<AddSubmissionRequest, Submission, AddSubmissionRequestToSubmissionMapper>();
            builder.Services.AddMapper<Domain.Spots.Spot, SpotResponse, SpotToSpotResponseMapper>();

            builder.Services.AddSingleton<ProblemDetailsFactory>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCorsPolicies();

            var app = builder.Build();

            app.UseExceptionHandlingMiddleware();

            if (app.Environment.IsDevelopment())
            {
                app.UseCors(CorsExtensions.DevelopmentPolicyName);
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
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
