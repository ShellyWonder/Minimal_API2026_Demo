namespace MinimalAPI2026Demo.Endpoints.IdentityEndpoints;

public static class CurrentUserEndpoints
{
    public static IEndpointRouteBuilder MapCurrentUserEndpoints(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes
            .MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapGet("/user-info", GetCurrentUser)
            .RequireAuthorization()
            .Produces<UserInfo>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", Logout)
            .Produces(StatusCodes.Status204NoContent);

        return routes;
    }

    private static async Task<Results<Ok<UserInfo>, UnauthorizedHttpResult>>
        GetCurrentUser(
            ClaimsPrincipal principal,
            ICurrentEmployeeService currentEmployeeService,
            ApplicationDbContext db,
            CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        if (employee is null)
        {
            return TypedResults.Unauthorized();
        }

        string? assignedSiteName = employee.AssignedSiteId is int siteId
            ? await db.Sites
                .AsNoTracking()
                .Where(s => s.Id == siteId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync(ct)
            : null;

        return TypedResults.Ok(new UserInfo
        {
            UserId = employee.User.Id,
            Email = employee.User.Email ?? string.Empty,
            FirstName = employee.User.FirstName,
            LastName = employee.User.LastName,
            Role = employee.Role,
            AssignedSiteId = employee.User.AssignedSiteId,
            AssignedSiteName = assignedSiteName
        });
    }

    private static async Task<NoContent> Logout(
        SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return TypedResults.NoContent();
    }
}
