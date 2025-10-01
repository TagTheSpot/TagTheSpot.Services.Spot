using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.Spot.Application.Options
{
    public sealed class SubmissionModerationSettings : IAppOptions
    {
        public static string SectionName => nameof(SubmissionModerationSettings);

        public bool Enabled { get; init; } = true;
    }
}
