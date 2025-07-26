using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;

namespace TagTheSpot.Services.Spot.Application.Abstractions.Services
{
    public interface ISpotService
    {
        Task<Domain.Spots.Spot?> GetByIdAsync(Guid id);

        Task<Result<Guid>> AddSpotAsync(AddSpotRequest request);
    }
}
