using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Repositories
{
    public sealed class SubmissionRepository : ISubmissionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SubmissionRepository> _logger;

        public SubmissionRepository(
            ApplicationDbContext dbContext, 
            ILogger<SubmissionRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Submission?> GetByIdAsync(
            Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Submissions
                .FindAsync(
                    keyValues: [id],
                    cancellationToken);
        }

        public async Task<IEnumerable<Submission>> GetByUserIdAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Submissions.Where(
                sub => sub.UserId == userId).ToListAsync();
        }

        public async Task UpdateAsync(
            Submission submission, 
            CancellationToken cancellationToken)
        {
            _dbContext.Submissions.Update(submission);

            var affected = await _dbContext.SaveChangesAsync(cancellationToken);

            if (affected == 0 || affected > 1)
            {
                _logger.LogWarning("Affected {AffectedRows} rows while updating a Submission entity with id {SubmissionId}", affected, submission.Id.ToString());
            }
        }
    }
}
