namespace TagTheSpot.Services.Spot.Domain.Submissions
{
    public interface ISubmissionRepository
    {
        Task InsertAsync(
            Submission submission, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Submission>> GetByUserIdAsync(
            Guid userId, CancellationToken cancellationToken = default);

        Task<Submission?> GetByIdAsync(
            Guid id, CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Submission submission, 
            CancellationToken cancellationToken = default);

        Task<bool> LocationForSubmissionTakenAsync(
            double latitude,
            double longitude,
            Guid cityId,
            double minDistanceBetweenSpotsInMeters);
    }
}
