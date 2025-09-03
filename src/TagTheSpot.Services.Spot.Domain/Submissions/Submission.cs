using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public sealed class Submission
    {
        public Guid Id { get; init; }

        public Guid UserId { get; set; }

        public Guid CityId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public SpotType Type { get; set; }

        public required string Description { get; set; }

        public SkillLevel? SkillLevel { get; set; }

        public bool? IsCovered { get; set; }

        public bool? Lighting { get; set; }

        public Accessibility? Accessibility { get; set; }

        public Condition? Condition { get; set; }

        public List<string> ImagesUrls { get; set; } = [];

        public SubmissionStatus Status { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
