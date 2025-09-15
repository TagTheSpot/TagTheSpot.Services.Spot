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

        public static readonly Error DescriptionIrrelevantOrContradictory =
            Error.Validation(
                code: "Submission.DescriptionIrrelevantOrContradictory",
                description: "The description is either irrelevant or contradicts the submission's fields.");

        public static readonly Error DescriptionInapproriate =
            Error.Validation(
                code: "Submission.DescriptionInapproriate",
                description: "The description contains inappropriate content.");

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
