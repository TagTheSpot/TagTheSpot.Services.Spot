using TagTheSpot.Services.Spot.Application.DTO;

namespace TagTheSpot.Services.Spot.Application.Abstractions.Services
{
    public interface ICityService
    {
        Task<IEnumerable<CityResponse>> GetMatchingCities(
            GetMatchingCitiesRequest request);
    }
}
