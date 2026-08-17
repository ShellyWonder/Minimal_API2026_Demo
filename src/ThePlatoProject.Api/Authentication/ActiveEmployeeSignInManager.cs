using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MinimalAPI2026Demo.Models;

namespace MinimalAPI2026Demo.Authentication;

public sealed class ActiveEmployeeSignInManager(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<ApplicationUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<ApplicationUser> confirmation)
    : SignInManager<ApplicationUser>(
        userManager,
        contextAccessor,
        claimsFactory,
        optionsAccessor,
        logger,
        schemes,
        confirmation)
{
    public override Task<bool> CanSignInAsync(ApplicationUser user)
    {
        if (!user.IsActive)
        {
            Logger.LogWarning(
                "Sign-in denied for deactivated employee {UserId}.",
                user.Id);

            return Task.FromResult(false);
        }

        return base.CanSignInAsync(user);
    }
}
