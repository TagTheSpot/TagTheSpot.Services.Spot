using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;

namespace TagTheSpot.Services.Spot.Application.Abstractions.Services
{
    public interface ISubmissionService
    {
        Task<Result<IEnumerable<SubmissionResponse>>> GetCurrentUserSubmissionsAsync(
            CancellationToken cancellationToken);

        Task<Result<SubmissionResponse?>> GetSubmissionByIdAsync(
            Guid id, CancellationToken cancellationToken);
    }
}
