using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;

namespace TagTheSpot.Services.Spot.Application.Mappers
{
    public sealed class SpotToSpotResponseMapper
        : Mapper<Domain.Spots.Spot, SpotResponse>
    {
        public override SpotResponse Map(Domain.Spots.Spot source)
        {
            return new SpotResponse(
                Id: source.Id,
                CityId: source.CityId,
                Latitude: source.Latitude,
                Longitude: source.Longitude,
                Type: source.Type,
                Description: source.Description,
                SkillLevel: source.SkillLevel,
                IsCovered: source.IsCovered,
                Lighting: source.Lighting,
                CreatedAt: source.CreatedAt,
                Accessibility: source.Accessibility,
                Condition: source.Condition,
                ImagesUrls: source.ImagesUrls);
        }
    }
}
