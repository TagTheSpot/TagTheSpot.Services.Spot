using Microsoft.AspNetCore.Http;

namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record AddSubmissionRequest(
        Guid CityId,
        double Latitude,
        double Longitude,
        string SpotType,
        string Description,
        string? SkillLevel,
        bool? IsCovered,
        bool? Lighting,
        string? Accessibility,
        string? Condition,
        List<IFormFile> Images);
}
