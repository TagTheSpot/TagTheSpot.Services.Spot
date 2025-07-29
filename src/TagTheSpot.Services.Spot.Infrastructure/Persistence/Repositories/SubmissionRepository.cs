using Microsoft.EntityFrameworkCore;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Repositories
{
    public sealed class SubmissionRepository : ISubmissionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SubmissionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Submission>> GetByUserIdAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Submissions.Where(
                sub => sub.UserId == userId).ToListAsync();
        }
    }
}
