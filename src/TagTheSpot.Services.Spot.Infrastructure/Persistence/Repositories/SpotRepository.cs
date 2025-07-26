using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Repositories
{
    public class SpotRepository : ISpotRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SpotRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Domain.Spots.Spot?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Spots
                .FindAsync(id);
        }

        public async Task InsertAsync(Domain.Spots.Spot spot)
        {
            await _dbContext.Spots.AddAsync(spot);
            await _dbContext.SaveChangesAsync();
        }
    }
}
