using TagTheSpot.Services.Shared.Abstractions.Mappers;
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
                Type: source.Type.ToString(),
                Description: source.Description,
                SkillLevel: source.SkillLevel.ToString(),
                IsCovered: source.IsCovered,
                Lighting: source.Lighting,
                CreatedAt: source.CreatedAt,
                Accessibility: source.Accessibility.ToString(),
                Condition: source.Condition.ToString(),
                ImagesUrls: source.ImagesUrls);
        }
    }
}
