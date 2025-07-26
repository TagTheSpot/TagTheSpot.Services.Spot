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

        public async Task<Domain.Spots.Spot?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Spots.FirstOrDefaultAsync(spot => spot.Id == id);
        }
    }
}
