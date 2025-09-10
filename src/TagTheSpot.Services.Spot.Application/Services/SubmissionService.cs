using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Shared.Messaging.Events.Submissions;
using TagTheSpot.Services.Spot.Application.Abstractions.AI;
using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Application.Abstractions.Geo;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.Abstractions.Storage;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Application.Options;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Domain.Submissions;
using TagTheSpot.Services.Spot.Domain.Users;

namespace TagTheSpot.Services.Spot.Application.Services
{
    public sealed class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICityRepository _cityRepository;
        private readonly IBlobService _blobService;
        private readonly IContentSafetyService _contentSafetyService;
        private readonly IGeoValidationService _geoValidationService;
        private readonly Mapper<AddSubmissionRequest, Submission> _requestMapper;
        private readonly Mapper<Submission, SubmissionResponse> _submissionMapper;
        private readonly ILogger<SubmissionService> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly LocationValidationSettings _locationValidationSettings;

        public SubmissionService(
            ISubmissionRepository submissionRepository,
            ICurrentUserService currentUserService,
            Mapper<Submission, SubmissionResponse> submissionMapper,
            ICityRepository cityRepository,
            ILogger<SubmissionService> logger,
            IUserRepository userRepository,
            Mapper<AddSubmissionRequest, Submission> requestMapper,
            IBlobService blobService,
            IContentSafetyService contentSafetyService,
            IPublishEndpoint publishEndpoint,
            IGeoValidationService geoValidationService,
            IOptions<LocationValidationSettings> locationValidationSettings)
        {
            _submissionRepository = submissionRepository;
            _currentUserService = currentUserService;
            _submissionMapper = submissionMapper;
            _cityRepository = cityRepository;
            _logger = logger;
            _requestMapper = requestMapper;
            _blobService = blobService;
            _publishEndpoint = publishEndpoint;
            _contentSafetyService = contentSafetyService;
            _geoValidationService = geoValidationService;
            _locationValidationSettings = locationValidationSettings.Value;
        }

        public async Task<Result<Guid>> AddSubmissionAsync(AddSubmissionRequest request)
        {
            var city = await _cityRepository.GetByIdAsync(request.CityId);

            if (city is null)
            {
                return Result.Failure<Guid>(SubmissionErrors.CityNotFound);
            }

            var isSpotWithinCity = _geoValidationService.IsCoordinateWithinCity(
                latitude: request.Latitude,
                longitude: request.Longitude,
                cityId: city.Id);

            if (!isSpotWithinCity)
            {
                return Result.Failure<Guid>(SubmissionErrors.SpotLocationOutsideCity);
            }

            var locationTaken = await _submissionRepository.LocationForSubmissionTakenAsync(
                latitude: request.Latitude,
                longitude: request.Longitude,
                cityId: request.CityId,
                minDistanceBetweenSpotsInMeters:
                    _locationValidationSettings.MinDistanceBetweenSpotsInMeters);

            if (locationTaken)
            {
                return Result.Failure<Guid>(SubmissionErrors.LocationAlreadyTaken);
            }

            var submission = _requestMapper.Map(request);

            submission.CityId = city.Id;

            var isDescriptionSafe = await _contentSafetyService
                .IsTextSafeAsync(submission.Description);

            if (!isDescriptionSafe)
            {
                return Result.Failure<Guid>(SubmissionErrors.DescriptionUnsafe);
            }

            List<string> imagesUris = [];

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

            submission.UserId = _currentUserService.GetUserId();

            await _submissionRepository.InsertAsync(submission);

            await _publishEndpoint.Publish(
                CreateSpotSubmittedEventObject(
                    cityName: city.Name,
                    submission));

            return Result.Success(submission.Id);
        }

        private static SpotSubmittedEvent CreateSpotSubmittedEventObject(
            string cityName, Submission submission)
        {
            return new SpotSubmittedEvent(
                SubmissionId: submission.Id,
                UserId: submission.UserId,
                CityId: submission.CityId,
                CityName: cityName,
                Latitude: submission.Latitude,
                Longitude: submission.Longitude,
                SpotType: submission.Type.ToString(),
                Description: submission.Description,
                ImagesUrls: submission.ImagesUrls,
                SubmittedAt: submission.SubmittedAt,
                IsCovered: submission.IsCovered,
                Lighting: submission.Lighting,
                SkillLevel: submission.SkillLevel.ToString(),
                Accessibility: submission.Accessibility.ToString(),
                Condition: submission.Condition.ToString());
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
