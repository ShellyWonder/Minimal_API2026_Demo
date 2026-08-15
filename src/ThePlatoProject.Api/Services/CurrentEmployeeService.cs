namespace MinimalAPI2026Demo.Services;

public sealed class CurrentEmployeeService(ApplicationDbContext db,
                                            UserManager<ApplicationUser> userManager)
                                            : ICurrentEmployeeService
{
    public async Task<CurrentEmployee?> GetAsync(ClaimsPrincipal principal,
                                                    CancellationToken ct)
    {
        string? userId = userManager.GetUserId(principal);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        ApplicationUser? user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        IList<string> roles = await userManager.GetRolesAsync(user);

        if (roles.Count != 1 || !AppRoles.AllRoles.Contains(roles[0]))
        {
            return null;
        }

        return new CurrentEmployee(user, roles[0]);
    }
}
