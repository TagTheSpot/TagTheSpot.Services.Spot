using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record SpotResponse(
        Guid Id,
        Guid CityId,
        double Latitude,
        double Longitude,
        SpotType Type,
        string Description,
        SkillLevel? SkillLevel,
        bool? IsCovered,
        bool? Lighting,
        DateTime CreatedAt,
        Accessibility? Accessibility,
        Condition? Condition,
        List<string> ImagesUrls);

    public static class SpotExtensions
    {
        public static SpotResponse ToSpotResponse(this Domain.Spots.Spot spot)
        {
            if (spot is null)
                throw new ArgumentNullException(nameof(spot));

            return new SpotResponse(
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
        }
    }
}
