using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;

namespace TagTheSpot.Services.Spot.Application.Abstractions.Services
{
    public interface ICityService
    {
        Task<IEnumerable<CityResponse>> GetMatchingCitiesAsync(
            GetMatchingCitiesRequest request);

        Task<Result<CityResponse>> GetByIdAsync(Guid id);
    }
}
