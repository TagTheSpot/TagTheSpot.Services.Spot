using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Spot.Application.Options
{
    public sealed class LocationValidationSettings
    {
        public const string SectionName = nameof(LocationValidationSettings);

        [Required]
        [Range(0, int.MaxValue)]
        public double MinDistanceBetweenSpotsInMeters { get; init; }
    }
}
