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
            Domain.Spots.Spot? spot = await _spotRepository.GetByIdAsync(id);

            if (spot is null)
            {
                return Result.Failure<SpotResponse>(Error
                    .NotFound("404", "Spot with the specified ID was not found."));
            }

            return Result.Success(spot.ToSpotResponse());
        }

        public async Task<Result<Guid>> DeleteSpotAsync(Guid id)
        {
            Domain.Spots.Spot? spot = await _spotRepository.GetByIdAsync(id);

            if (spot is null)
                return Result.Failure<Guid>(Error.NotFound("404", "Spot does not exist"));

            await _spotRepository.DeleteAsync(spot);

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

            return Result.Success(spots.Select(spot => spot.ToSpotResponse()).ToList());
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
    }
}
