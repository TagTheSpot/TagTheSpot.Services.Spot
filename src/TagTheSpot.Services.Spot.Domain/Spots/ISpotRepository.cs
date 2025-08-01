namespace TagTheSpot.Services.Spot.Domain.Spots
{
    public interface ISpotRepository
    {
        Task<Spot?> GetByIdAsync(
            Guid id, 
            CancellationToken cancellationToken = default);

        Task InsertAsync(
            Spot spot, 
            CancellationToken cancellationToken = default); 
    }
}
