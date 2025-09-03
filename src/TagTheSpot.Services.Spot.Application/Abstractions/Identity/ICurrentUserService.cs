using TagTheSpot.Services.Spot.Domain.Users;

namespace TagTheSpot.Services.Spot.Application.Abstractions.Identity
{
    public interface ICurrentUserService
    {
        Guid GetUserId();

        Role GetRole();
    }
}
