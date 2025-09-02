namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record SpotResponse(
        Guid Id,
        Guid CityId,
        double Latitude,
        double Longitude,
        string Type,
        string Description,
        DateTime CreatedAt,
        List<string> ImagesUrls,
        string? SkillLevel,
        bool? IsCovered,
        bool? Lighting,
        string? Accessibility,
        string? Condition);
}
