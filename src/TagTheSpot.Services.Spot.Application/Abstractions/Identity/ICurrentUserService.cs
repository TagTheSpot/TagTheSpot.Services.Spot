namespace TagTheSpot.Services.Spot.Application.Abstractions.Identity
{
    public interface ICurrentUserService
    {
        Guid GetCurrentUserId();
    }
}
