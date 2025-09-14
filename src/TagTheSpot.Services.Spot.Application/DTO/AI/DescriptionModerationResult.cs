namespace TagTheSpot.Services.Spot.Application.DTO.AI
{
    public sealed record DescriptionModerationResult(
        bool IsRelevant,
        bool IsAppropriate);
}
