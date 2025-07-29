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
}
