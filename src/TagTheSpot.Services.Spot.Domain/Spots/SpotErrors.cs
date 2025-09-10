using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Spot.Domain.Spots
{
    public static class SpotErrors
    {
        public static readonly Error NotFound =
            Error.NotFound(
                code: "Spot.NotFound",
                description: "The spot has not been found.");

        public static readonly Error CityNotFound =
            Error.NotFound(
                code: "Spot.CityNotFound",
                description: "The city has not been found.");

        public static readonly Error LocationOutsideCity =
            Error.Validation(
                code: "Spot.LocationOutsideCity",
                description: "The coordinates of the spot are outside of the requested city.");

        public static readonly Error LocationAlreadyTaken =
            Error.Validation(
                code: "Spot.LocationAlreadyTaken",
                description: "The coordinates of the spot are too close to another existing spot/submission.");
    }
}
