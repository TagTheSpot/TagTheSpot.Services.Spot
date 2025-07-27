namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record AddSpotRequest(
        Guid CityId,
        double Latitude,
        double Longitude,
        string SpotType,
        string Description,
        bool? IsCovered,
        bool? Lighting,
        string? SkillLevel,
        string? Accessibility,
        string? Condition);
}