using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Spot.Application.Options
{
    public sealed class SubmissionModerationSettings
    {
        public const string SectionName = nameof(SubmissionModerationSettings);

        [Required]
        public required bool Enabled { get; init; } = true;
    }
}
