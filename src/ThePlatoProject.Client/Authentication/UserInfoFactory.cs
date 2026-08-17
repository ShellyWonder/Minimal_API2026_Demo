using System.Security.Claims;
using ThePlatoProject.Contracts.Authentication;

namespace ThePlatoProject.Client.Authentication;

public static class UserInfoFactory
{
    public static UserInfo? FromPrincipal(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
        string? role = principal.FindFirst(ClaimTypes.Role)?.Value;
        string? firstName = principal.FindFirst(nameof(UserInfo.FirstName))?.Value;
        string? lastName = principal.FindFirst(nameof(UserInfo.LastName))?.Value;

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(role))
        {
            return null;
        }

        int? assignedSiteId = int.TryParse(
            principal.FindFirst(nameof(UserInfo.AssignedSiteId))?.Value,
            out int parsedSiteId)
                ? parsedSiteId
                : null;

        return new UserInfo
        {
            UserId = userId,
            Email = email,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            Role = role,
            AssignedSiteId = assignedSiteId,
            AssignedSiteName = principal.FindFirst(nameof(UserInfo.AssignedSiteName))?.Value
        };
    }
}
