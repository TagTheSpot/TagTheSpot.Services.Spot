using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
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

        public async Task InsertAsync(
            Submission submission, 
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Submissions.AddAsync(submission, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Submission?> GetByIdAsync(
            Guid id, 
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Submissions
                .FindAsync(
                    keyValues: [id],
                    cancellationToken);
        }

        public async Task<IEnumerable<Submission>> GetByUserIdAsync(
            Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Submissions.Where(
                sub => sub.UserId == userId).ToListAsync();
        }

        public async Task UpdateAsync(
            Submission submission, 
            CancellationToken cancellationToken = default)
        {
            _dbContext.Submissions.Update(submission);

            var affected = await _dbContext.SaveChangesAsync(cancellationToken);

            if (affected == 0 || affected > 1)
            {
                _logger.LogWarning("Affected {AffectedRows} rows while updating a Submission entity with id {SubmissionId}", affected, submission.Id.ToString());
            }
        }

        public async Task<bool> LocationForSubmissionTakenAsync(
            double latitude, 
            double longitude,
            Guid cityId, 
            double minDistanceBetweenSpotsInMeters)
        {
            var point = new Point(longitude, latitude) { SRID = 4326 };
            const bool useSpheroid = true;

            return await _dbContext.Submissions
                .Where(s => s.CityId == cityId &&
                    EF.Functions.IsWithinDistance(point, EF.Property<Point>(s, "Location"), minDistanceBetweenSpotsInMeters, useSpheroid))
                .Select(s => true)
                .Concat(
                    _dbContext.Spots
                        .Where(s => s.CityId == cityId &&
                            EF.Functions.IsWithinDistance(point, EF.Property<Point>(s, "Location"), minDistanceBetweenSpotsInMeters, useSpheroid))
                        .Select(s => true)
                )
                .AnyAsync();
        }
    }
}
