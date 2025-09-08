using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public static class SubmissionErrors
    {
        public static readonly Error CityNotFound =
            Error.NotFound(
                code: "Submission.CityNotFound",
                description: "The city has not been found");
      
        public static readonly Error NotFound =
            Error.NotFound(
                code: "Submission.NotFound",
                description: "The submission has not been found.");

        public static readonly Error DescriptionUnsafe =
            Error.Validation(
                code: "Submission.DescriptionUnsafe",
                description: "The description contains unsafe content.");
    }
}
