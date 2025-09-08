namespace TagTheSpot.Services.Spot.Application.Abstractions.AI
{
    public interface IContentSafetyService
    {
        Task<bool> IsTextSafeAsync(string text);
    }
}
