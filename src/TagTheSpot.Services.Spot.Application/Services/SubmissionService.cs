using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Submissions;
using TagTheSpot.Services.Spot.Domain.Users;

namespace TagTheSpot.Services.Spot.Application.Services
{
    public sealed class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly Mapper<Submission, SubmissionResponse> _submissionMapper;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            Mapper<Submission, SubmissionResponse> submissionMapper)
        {
            _submissionRepository = submissionRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _submissionMapper = submissionMapper;
        }

        public async Task<Result<IEnumerable<SubmissionResponse>>> GetCurrentUserSubmissionsAsync(
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            var submissions = await _submissionRepository
                .GetByUserIdAsync(userId, cancellationToken);

            return Result.Success(_submissionMapper.Map(submissions));
        }

        public async Task<Result<SubmissionResponse?>> GetSubmissionByIdAsync(
            Guid id, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var role = _currentUserService.GetRole();

            var submission = await _submissionRepository.GetByIdAsync(id, cancellationToken);

            if (submission is null)
            {
                return Result.Failure<SubmissionResponse?>(SubmissionErrors.NotFound);
            }

            if (role == Role.RegularUser && submission.UserId != userId)
            {
                return Result.Failure<SubmissionResponse?>(SubmissionErrors.NotFound); 
            }

            return _submissionMapper.Map(submission);
        }
    }
}
