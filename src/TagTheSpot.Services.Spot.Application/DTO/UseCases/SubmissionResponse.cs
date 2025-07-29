namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record SubmissionResponse(
        Guid Id,
        Guid UserId,
        Guid SpotId,
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
        List<string> ImagesUrls,
        string SubmissionStatus,
        string? RejectionReason,
        DateTime SubmittedAt);
}
