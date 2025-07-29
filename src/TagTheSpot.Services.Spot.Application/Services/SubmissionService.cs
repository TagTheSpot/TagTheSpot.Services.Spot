using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Services
{
    public sealed class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly Mapper<Submission, SubmissionResponse> _submissionMapper;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            ICurrentUserService currentUserService,
            Mapper<Submission, SubmissionResponse> submissionMapper)
        {
            _submissionRepository = submissionRepository;
            _currentUserService = currentUserService;
            _submissionMapper = submissionMapper;
        }

        public async Task<Result<IEnumerable<SubmissionResponse>>> GetCurrentUserSubmissionsAsync(
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetCurrentUserId();

            var submissions = await _submissionRepository
                .GetByUserIdAsync(userId, cancellationToken);

            return Result.Success(_submissionMapper.Map(submissions));
        }
    }
}
