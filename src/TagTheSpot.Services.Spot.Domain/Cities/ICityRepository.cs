namespace TagTheSpot.Services.Spot.Domain.Cities
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetMatchingCitiesAsync(string cityPattern);
    }
}
