using TagTheSpot.Services.Shared.Abstractions.Mappers;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Application.Mappers
{
    public sealed class AddSpotRequestToSpotMapper
        : Mapper<AddSpotRequest, Domain.Spots.Spot>
    {
        public override Domain.Spots.Spot Map(AddSpotRequest source)
        {
            return new Domain.Spots.Spot()
            {
                Id = Guid.NewGuid(),
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                Type = Enum.Parse<SpotType>(source.SpotType),
                Description = source.Description,
                Lighting = source.Lighting,
                IsCovered = source.IsCovered,
                Accessibility = source.Accessibility is null ? Accessibility.Unknown : Enum.Parse<Accessibility>(source.Accessibility),
                Condition = source.Condition is null ? null : Enum.Parse<Condition>(source.Condition),
                SkillLevel = source.SkillLevel is null ? null : Enum.Parse<SkillLevel>(source.SkillLevel),
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
