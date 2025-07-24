namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public class Submission
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public SubmissionStatus Status { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
