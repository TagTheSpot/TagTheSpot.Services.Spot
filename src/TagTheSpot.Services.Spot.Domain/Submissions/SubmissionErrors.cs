using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public static class SubmissionErrors
    {
        public static readonly Error NotFound =
            Error.NotFound(
                code: "Submission.NotFound",
                description: "The submission has not been found.");
    }
}
