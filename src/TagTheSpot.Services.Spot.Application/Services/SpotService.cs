using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Shared.Essentials.Results;
using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.Abstractions.Storage;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Application.Services
{
    public class SpotService : ISpotService
    {
        private readonly ISpotRepository _spotRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IBlobService _blobService;
        private readonly Mapper<AddSpotRequest, Domain.Spots.Spot> _requestMapper;
        private readonly Mapper<Domain.Spots.Spot, SpotResponse> _responseMapper;
        private readonly ILogger<SpotService> _logger;

        public SpotService(
            ISpotRepository spotRepository,
            ICityRepository cityRepository,
            IBlobService blobService,
            Mapper<AddSpotRequest, Domain.Spots.Spot> requestMapper,
            Mapper<Domain.Spots.Spot, SpotResponse> responseMapper,
            ILogger<SpotService> logger)
        {
            _spotRepository = spotRepository;
            _cityRepository = cityRepository;
            _blobService = blobService;
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
            _logger = logger;
        }

        public async Task<Result<Guid>> AddSpotAsync(AddSpotRequest request)
        {
            var spot = _requestMapper.Map(request);

            var cityExists = await _cityRepository.ExistsAsync(request.CityId);

            if (!cityExists)
            {
                return Result.Failure<Guid>(SpotErrors.CityNotFound);
            }

            spot.CityId = request.CityId;

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
                    _logger.LogError(ex, "Fail during uploading image with name {ImageName} for spot ID {SpotId}. Error: {ExceptionError}", image.FileName, spot.Id, ex.Message);
                }
            }

            spot.ImagesUrls = imagesUris;

            await _spotRepository.InsertAsync(spot);

            return Result.Success(spot.Id);
        }

        public async Task<Result<SpotResponse>> GetByIdAsync(Guid id)
        {
            var spot = await _spotRepository.GetByIdAsync(id);

            if (spot is null)
            {
                return Result.Failure<SpotResponse>(SpotErrors.NotFound);
            }

            return Result.Success(_responseMapper.Map(spot));
        }

        public async Task<Result<Guid>> DeleteSpotAsync(Guid id)
        {
            var spot = await _spotRepository.GetByIdAsync(id);

            if (spot is null)
            {
                return Result.Failure<Guid>(SpotErrors.NotFound);
            }

            await _spotRepository.DeleteAsync(spot);

            foreach (var imageUrl in spot.ImagesUrls)
            {
                try
                {
                    await _blobService.DeleteAsync(
                        blobUri: imageUrl);

                    _logger.LogInformation("Successfully deleted blob for spot {SpotId} with URL: {ImageUrl}.",
                        spot.Id, imageUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,"Failed to delete blob for spot {SpotId} with URL: {ImageUrl}.",
                        spot.Id, imageUrl);
                }
            }

            return Result.Success(spot.Id);
        }

        public async Task<Result<List<SpotResponse>>> GetByCityIdAsync(Guid cityId)
        {
            var exists = await _cityRepository.ExistsAsync(cityId);

            if (!exists)
            {
                return Result.Failure<List<SpotResponse>>(SpotErrors.CityNotFound);
            }

            List<Domain.Spots.Spot> spots = await _spotRepository
                .GetByCityIdAsync(cityId);

            return Result.Success(_responseMapper.Map(spots).ToList());
        }

        public async Task<Result<List<SpotResponse>>> GetRandomByCityIdAsync(
            GetRandomSpotsByCityIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var exists = await _cityRepository.ExistsAsync(request.CityId);

            if (!exists)
            {
                return Result.Failure<List<SpotResponse>>(SpotErrors.CityNotFound);
            }

            var spots = await _spotRepository.GetRandomByCityIdAsync(
                cityId: request.CityId,
                count: request.Count,
                cancellationToken);

            return _responseMapper.Map(spots).ToList();
        }

        public async Task<Result<List<CoordinatesResponse>>> GetCoordinatesByCityIdAsync(Guid cityId)
        {
            var exists = await _cityRepository.ExistsAsync(cityId);

            if (!exists)
            {
                return Result.Failure<List<CoordinatesResponse>>(SpotErrors.CityNotFound);
            }

            var spots = await _spotRepository
                .GetByCityIdAsync(cityId);

            var coordinates = spots
                .Select(spot => new CoordinatesResponse(
                    Latitude: spot.Latitude,
                    Longitude: spot.Longitude))
                .ToList();

            return Result.Success(coordinates);
        }
    }
}
