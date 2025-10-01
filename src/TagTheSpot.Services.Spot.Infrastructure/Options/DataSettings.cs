using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Options
{
    public sealed class DataSettings : IAppOptions
    {
        public static string SectionName => nameof(DataSettings);

        [Required]
        public required string CityDataRelativePath { get; init; }

        [Required]
        public required string UAGeoJsonRelativePath { get; init; }
    }
}
