using TagTheSpot.Services.Spot.Application.DTO.AI;

namespace TagTheSpot.Services.Spot.Application.Abstractions.AI
{
    public interface ISpotModerationService
    {
        Task<DescriptionModerationResult> ModerateDescriptionAsync(Domain.Spots.Spot spot);
    }
}
