
using FluentValidation;
using FluentValidation.AspNetCore;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.Services;
using TagTheSpot.Services.Spot.Application.Validators;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Infrastructure.Options;
using TagTheSpot.Services.Spot.Infrastructure.Persistence;
using TagTheSpot.Services.Spot.Infrastructure.Persistence.Options;

namespace TagTheSpot.Services.Spot.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOptions<DbSettings>()
                .BindConfiguration(DbSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<DataSettings>()
                .BindConfiguration(DataSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddSingleton<ICityRepository, CityRepository>();
            builder.Services.AddScoped<ICityService, CityService>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<GetMatchingCitiesRequestValidator>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
