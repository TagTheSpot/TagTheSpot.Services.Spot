using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Abstractions.Services
{
    public interface ISubmissionService
    {
        Task<Result<Guid>> AddSubmissionAsync(AddSubmissionRequest request);

        Task<Result<IEnumerable<SubmissionResponse>>> GetCurrentUserSubmissionsAsync(
            CancellationToken cancellationToken);
    }
}
