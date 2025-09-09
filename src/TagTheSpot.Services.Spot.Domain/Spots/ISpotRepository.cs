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

        Task DeleteAsync(
            Spot spot,
            CancellationToken cancellationToken = default);

        Task<List<Spot>> GetByCityIdAsync(
            Guid cityId,
            CancellationToken cancellationToken = default);

        Task<List<Spot>> GetRandomByCityIdAsync(
            Guid cityId,
            int count,
            CancellationToken cancellationToken = default);

        Task<bool> SpotsExistNearbyAsync(
            double latitude,
            double longitude,
            Guid cityId,
            double minDistanceBetweenSpotsInMeters);
    }
}
