namespace TagTheSpot.Services.Spot.Application.Options
{
    public sealed class SubmissionModerationSettings
    {
        public const string SectionName = nameof(SubmissionModerationSettings);

        public bool Enabled { get; init; } = true;
    }
}
