namespace TagTheSpot.Services.Spot.Domain.Users
{
    public interface IUserRepository
    {
        Task InsertAsync(
            User user, 
            CancellationToken cancellationToken);
    }
}
