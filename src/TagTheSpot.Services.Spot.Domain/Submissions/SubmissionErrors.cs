using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public static class SubmissionErrors
    {
        public static readonly Error CityNotFound =
            Error.NotFound(
                code: "Submission.CityNotFound",
                description: "The city has not been found");
    }
}
