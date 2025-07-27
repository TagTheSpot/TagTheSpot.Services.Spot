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
        private readonly Mapper<AddSpotRequest, Domain.Spots.Spot> _mapper;

        public SpotService(
            ISpotRepository spotRepository,
            ICityRepository cityRepository,
            Mapper<AddSpotRequest, Domain.Spots.Spot> mapper)
        {
            _spotRepository = spotRepository;
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> AddSpotAsync(AddSpotRequest request)
        {
            var spot = _mapper.Map(request);

            var cityExists = await _cityRepository.ExistsAsync(request.CityId);

            if (!cityExists)
            {
                return Result.Failure<Guid>(SpotErrors.CityNotFound);
            }

            spot.CityId = request.CityId;

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

            SpotResponse spotResponse = new SpotResponse(
                Id: spot.Id,
                CityId: spot.CityId,
                ImagesUrls: spot.ImagesUrls,
                Latitude: spot.Latitude,
                Longitude: spot.Longitude,
                Type: spot.Type,
                Description: spot.Description,
                SkillLevel: spot.SkillLevel,
                IsCovered: spot.IsCovered,
                Lighting: spot.Lighting,
                CreatedAt: spot.CreatedAt,
                Accessibility: spot.Accessibility,
                Condition: spot.Condition);

            return Result.Success(spotResponse);
        }
    }
}
