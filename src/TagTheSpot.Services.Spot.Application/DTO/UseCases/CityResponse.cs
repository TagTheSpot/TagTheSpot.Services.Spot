using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record CityResponse(
        Guid Id,
        string Name,
        double Latitude,
        double Longitude);

    public static class CityExtensions
    {
        public static CityResponse ToCityResponse(this City city)
        {
            if (city is null)
                throw new ArgumentNullException(nameof(city));

            return new CityResponse(
                Id: city.Id,
                Name: city.Name,
                Latitude: city.Latitude,
                Longitude: city.Longitude);
        }
    }
}
