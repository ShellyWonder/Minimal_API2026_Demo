namespace MinimalAPI2026Demo.Middleware;

public sealed class ActiveEmployeeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            ApplicationUser? user = await userManager.GetUserAsync(
                context.User);

            IList<string> roles = user is null
                ? []
                : await userManager.GetRolesAsync(user);

            if (user is null ||
                !user.IsActive ||
                roles.Count != 1 ||
                !AppRoles.AllRoles.Contains(roles[0]))
            {
                await signInManager.SignOutAsync();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await next(context);
    }
}
