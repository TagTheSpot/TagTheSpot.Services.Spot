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

        public async Task<Domain.Spots.Spot?> GetByIdAsync(Guid id)
        {
            return await _spotRepository.GetByIdAsync(id);
        }
    }
}
