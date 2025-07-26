namespace TagTheSpot.Services.Spot.Application.Abstractions.Services
{
    public interface ISpotService
    {
        Task<Domain.Spots.Spot?> GetByIdAsync(Guid id);
    }
}
