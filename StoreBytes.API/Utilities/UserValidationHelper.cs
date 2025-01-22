using Microsoft.AspNetCore.Mvc;
using StoreBytes.DataAccess.Interfaces;
using System.Security.Claims;

namespace StoreBytes.API.Utilities
{
    public static class UserValidationHelper
    {
        public static IActionResult ValidateUser(ClaimsPrincipal user, out int userId)
        {
            userId = 0;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out userId))
            {
                return new UnauthorizedObjectResult(new { error = "Invalid or missing user ID." });
            }

            return null;
        }

        public static IActionResult ValidateOwnership(IOwnable entity, ClaimsPrincipal user)
        {
            var validationResponse = ValidateUser(user, out int userId);
            if (validationResponse != null)
            {
                return validationResponse;
            }

            if (entity.UserId != userId)
            {
                return new UnauthorizedObjectResult(new { error = "You do not have access to this resource." });
            }

            return null;
        }
    }
}
