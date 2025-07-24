using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Spot.Infrastructure.Options
{
    public sealed class DataSettings
    {
        public const string SectionName = nameof(DataSettings);

        [Required]
        public required string CityDataRelativePath { get; init; }
    }
}
