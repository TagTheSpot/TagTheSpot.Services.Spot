using TagTheSpot.Services.Shared.Essentials.Results;

namespace TagTheSpot.Services.Spot.Domain.Spots
{
    public static class SpotErrors
    {
        public static readonly Error CityNotFound =
            Error.NotFound(
                code: "Spot.CityNotFound",
                description: "The city has not been found.");
    }
}
