using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Spot.Application.Options
{
    public sealed class SpotModerationSettings
    {
        public const string SectionName = nameof(SpotModerationSettings);

        [Required]
        public required bool Enabled { get; init; } = true;
    }
}
