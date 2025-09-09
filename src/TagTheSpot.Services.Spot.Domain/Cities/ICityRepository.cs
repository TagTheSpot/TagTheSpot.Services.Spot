namespace TagTheSpot.Services.Spot.Domain.Cities
{
    public interface ICityRepository
    {
        Task<List<City>> GetMatchingCitiesAsync(string cityPattern);

        Task<City?> GetByIdAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);

        Task<List<City>> GetAllAsync();
    }
}
