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

        public static readonly Error SpotLocationOutsideCity =
            Error.Validation(
                code: "Submission.SpotLocationOutsideCity",
                description: "The coordinates of the location are outside of the requested city.");

        public static readonly Error LocationAlreadyTaken =
            Error.Validation(
                code: "Submission.LocationAlreadyTaken",
                description: "The coordinates of the location are too close to another existing spot/submission.");
    }
}
