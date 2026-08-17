namespace MinimalAPI2026Demo.Extensions.ProgramExtensions;

public static class IdentityExtensions
{

    public static IServiceCollection AddIdentityAndAuthentication(this IServiceCollection services)
    {
        // Add Identity services
        services.AddIdentityApiEndpoints<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddRoles<IdentityRole>()
        .AddSignInManager<ActiveEmployeeSignInManager>()
        .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddPlatoAuthorization();

        //Admin policy
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        //enable validation
        services.AddValidation();
        return services;
    }

    public static IServiceCollection AddPlatoAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.ManageSites, policy =>
                policy.RequireRole(AppRoles.Admin, AppRoles.SiteManager))
            .AddPolicy(AppPolicies.ViewPrivateArtifacts, policy =>
                policy.RequireRole(AppRoles.AllRoles.ToArray()))
            .AddPolicy(AppPolicies.VerifyArtifactMedia, policy =>
                policy.RequireRole(AppRoles.Admin, AppRoles.SiteManager))
            .AddPolicy(AppPolicies.ManageEmployees, policy =>
                policy.RequireRole(AppRoles.Admin))
            .AddPolicy(AppPolicies.ViewAuditLog, policy =>
                policy.RequireRole(AppRoles.Admin));

        services.AddScoped<ICurrentEmployeeService, CurrentEmployeeService>();
        services.AddScoped<IPlatoAuthorizationService, PlatoAuthorizationService>();

        return services;
    }

}
