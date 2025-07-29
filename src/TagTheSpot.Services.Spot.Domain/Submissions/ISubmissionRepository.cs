namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public interface ISubmissionRepository
    {
        Task<IEnumerable<Submission>> GetByUserIdAsync(
            Guid userId, CancellationToken cancellationToken);
    }
}
