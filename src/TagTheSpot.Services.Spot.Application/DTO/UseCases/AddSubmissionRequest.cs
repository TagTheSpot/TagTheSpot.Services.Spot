using Microsoft.AspNetCore.Http;
using TagTheSpot.Services.Spot.Domain.Spots;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record AddSubmissionRequest(
        Guid UserId,
        Guid SpotId,
        Guid CityId,
        double Latitude,
        double Longitude,
        string Type,
        string Description,
        string? SkillLevel,
        bool? IsCovered,
        bool? Lighting,
        string? Accessibility,
        string? Condition,
        List<IFormFile> Images);
}
