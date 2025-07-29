using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;

namespace TagTheSpot.Services.Spot.Infrastructure.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;

            if (userClaims?.Identity is null || !userClaims.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var idClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                throw new InvalidOperationException($"Claim missing: id.");
            }

            var idString = idClaim.Value;

            if (Guid.TryParse(idString, out Guid id))
            {
                return id;
            }

            throw new InvalidOperationException("Failed to parse ID to GUID.");
        }
    }
}
