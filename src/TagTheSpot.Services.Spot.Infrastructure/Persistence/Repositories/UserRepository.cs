using TagTheSpot.Services.Spot.Domain.Users;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertAsync(
            User user, 
            CancellationToken cancellationToken)
        {
            await _dbContext.Users.AddAsync(
                user, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
