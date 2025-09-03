using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TagTheSpot.Services.Spot.Application.Abstractions.Identity;
using TagTheSpot.Services.Spot.Domain.Users;

namespace TagTheSpot.Services.Spot.Infrastructure.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Role GetRole()
        {
            var claimValue = GetClaim(ClaimTypes.Role).Value;

            if (string.IsNullOrWhiteSpace(claimValue))
            {
                throw new InvalidOperationException(
                    "The role claim is empty, even though the user is authenticated.");
            }

            return Enum.Parse<Role>(claimValue);
        }

        public Guid GetUserId()
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

        private Claim GetClaim(string claimType)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;

            if (userClaims?.Identity is null || !userClaims.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var claim = userClaims.FindFirst(ClaimTypes.Role);

            if (claim is null)
            {
                throw new InvalidOperationException($"Claim missing: {claimType}.");
            }

            return claim;
        }
    }
}
