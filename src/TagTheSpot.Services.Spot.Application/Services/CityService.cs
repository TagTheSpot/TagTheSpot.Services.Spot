using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO;
using TagTheSpot.Services.Spot.Domain.Cities;

namespace TagTheSpot.Services.Spot.Application.Services
{
    public sealed class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<CityResponse>> GetMatchingCities(
            GetMatchingCitiesRequest request)
        {
            var matchingCities = await _cityRepository.GetMatchingCities(request.CityPattern);

            return matchingCities.Select(
                city => new CityResponse(
                    Id: city.Id,
                    Name: city.Name,
                    Latitude: city.Latitude,
                    Longitude: city.Longitude));
        }
    }
}
