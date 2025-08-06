using Microsoft.EntityFrameworkCore;
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

        public async Task<Domain.Spots.Spot?> GetByIdAsync(
            Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Spots
                .FindAsync(
                    keyValues: [id],
                    cancellationToken);
        }

        public async Task InsertAsync(
            Domain.Spots.Spot spot,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Spots.AddAsync(spot, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Domain.Spots.Spot spot,
            CancellationToken cancellationToken = default)
        {
            _dbContext.Spots.Remove(spot);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Domain.Spots.Spot>> GetByCityIdAsync(Guid cityId, CancellationToken cancellationToken = default)
        {
            IQueryable<Domain.Spots.Spot> spots = _dbContext.Spots
                .Where(spot => spot.CityId == cityId);

            return await spots.ToListAsync();
        }
    }
}
