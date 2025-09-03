namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public interface ISubmissionRepository
    {
        Task InsertAsync(
            Submission submission, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Submission>> GetByUserIdAsync(
            Guid userId, CancellationToken cancellationToken);

        Task<Submission?> GetByIdAsync(
            Guid id, CancellationToken cancellationToken);

        Task UpdateAsync(
            Submission submission, 
            CancellationToken cancellationToken);
    }
}
