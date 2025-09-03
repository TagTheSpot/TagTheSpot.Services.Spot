using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.Abstractions.Storage;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Domain.Spots;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Services
{
    public sealed class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICityRepository _cityRepository;
        private readonly IBlobService _blobService;
        private readonly Mapper<AddSubmissionRequest, Submission> _requestMapper;
        private readonly Mapper<Submission, SubmissionResponse> _submissionMapper;
        private readonly ILogger<SpotService> _logger;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            ICurrentUserService currentUserService,
            Mapper<Submission, SubmissionResponse> submissionMapper,
            ICityRepository cityRepository,
            ILogger<SpotService> logger)
        {
            _submissionRepository = submissionRepository;
            _currentUserService = currentUserService;
            _submissionMapper = submissionMapper;
            _cityRepository = cityRepository;
            _logger = logger;
        }

        public async Task<Result<Guid>> AddSubmissionAsync(AddSubmissionRequest request)
        {
            var cityExists = await _cityRepository.ExistsAsync(request.CityId);

            if (!cityExists)
            {
                return Result.Failure<Guid>(SubmissionErrors.CityNotFound);
            }

            var submission = _requestMapper.Map(request);

            submission.CityId = request.CityId;

            List<string> imagesUris = new();

            foreach (var image in request.Images)
            {
                try
                {
                    using Stream stream = image.OpenReadStream();

                    var uploadResponse = await _blobService.UploadAsync(
                        stream,
                        contentType: image.ContentType);

                    imagesUris.Add(uploadResponse.BlobUri);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fail during uploading image with name {ImageName} for spot ID {SpotId}. Error: {ExceptionError}", image.FileName, submission.Id, ex.Message);
                }
            }

            submission.ImagesUrls = imagesUris;

            submission.UserId = _currentUserService.GetCurrentUserId();

            await _submissionRepository.InsertAsync(submission);

            return Result.Success(submission.Id);
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
