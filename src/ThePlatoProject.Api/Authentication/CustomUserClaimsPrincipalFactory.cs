using Microsoft.Extensions.Options;

namespace MinimalAPI2026Demo.Authentication;

public sealed class CustomUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(
        userManager,
        roleManager,
        options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(
        ApplicationUser user)
    {
        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim(
            nameof(UserInfo.FirstName),
            user.FirstName));

        identity.AddClaim(new Claim(
            nameof(UserInfo.LastName),
            user.LastName));

        if (!string.IsNullOrWhiteSpace(user.Email) &&
            !identity.HasClaim(claim => claim.Type == ClaimTypes.Email))
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        }

        if (user.AssignedSiteId is int assignedSiteId)
        {
            identity.AddClaim(new Claim(
                nameof(UserInfo.AssignedSiteId),
                assignedSiteId.ToString()));
        }

        // base.GenerateClaimsAsync already creates Identity role claims.
        return identity;
    }
}
