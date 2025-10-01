using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.Spot.Application.Options
{
    public sealed class LocationValidationSettings : IAppOptions
    {
        public static string SectionName => nameof(LocationValidationSettings);

        [Required]
        [Range(0, int.MaxValue)]
        public double MinDistanceBetweenSpotsInMeters { get; init; }
    }
}
