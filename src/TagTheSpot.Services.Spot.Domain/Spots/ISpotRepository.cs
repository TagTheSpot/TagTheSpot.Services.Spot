namespace TagTheSpot.Services.Spot.Domain.Spots
{
    public interface ISpotRepository
    {
        Task<Spot?> GetByIdAsync(Guid id);

        Task InsertAsync(Spot spot); 
    }
}
