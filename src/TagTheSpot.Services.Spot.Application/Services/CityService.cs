using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
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

        public async Task<Result<CityResponse>> GetByIdAsync(Guid id)
        {
            City? city = _cityRepository.GetByIdAsync(id).Result;

            if (city is null)
            {
                return Result.Failure<CityResponse>(Error
                    .NotFound("404", "City with the specified ID was not found."));
            }

            return Result.Success(city.ToCityResponse());
        }

        public async Task<IEnumerable<CityResponse>> GetMatchingCitiesAsync(
            GetMatchingCitiesRequest request)
        {
            var matchingCities = await _cityRepository.GetMatchingCitiesAsync(request.Pattern);

            return matchingCities.Select(
                city => new CityResponse(
                    Id: city.Id,
                    Name: city.Name,
                    Latitude: city.Latitude,
                    Longitude: city.Longitude));
        }
    }
}
