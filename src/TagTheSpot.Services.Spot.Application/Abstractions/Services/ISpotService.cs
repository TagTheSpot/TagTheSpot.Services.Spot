using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;

namespace TagTheSpot.Services.Spot.Application.Abstractions.Services
{
    public interface ISpotService
    {
        Task<Result<SpotResponse>> GetByIdAsync(Guid id);

        Task<Result<Guid>> AddSpotAsync(AddSpotRequest request);

        Task<Result<Guid>> DeleteSpotAsync(Guid id);

        Task<Result<List<SpotResponse>>> GetByCityId(Guid cityId);
    }
}
