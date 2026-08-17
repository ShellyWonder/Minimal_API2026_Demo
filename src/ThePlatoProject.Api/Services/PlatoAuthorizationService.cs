using MinimalAPI2026Demo.Authorization;

namespace MinimalAPI2026Demo.Services;

public class PlatoAuthorizationService(ApplicationDbContext db,
                                        ICurrentEmployeeService currentEmployeeService)
                                        : IPlatoAuthorizationService
{
    public async Task<bool> CanViewSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        if (employee is null ||
            !await db.Sites.AsNoTracking().AnyAsync(s => s.Id == siteId, ct))
        {
            return false;
        }

        return HasOrganizationWideReadScope(employee) ||
               IsAssignedToSite(employee, siteId);
    }

    public async Task<bool> CanManageSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        if (employee is null)
        {
            return false;
        }

        SiteLifecycleStatus? state = await db.Sites
            .AsNoTracking()
            .Where(s => s.Id == siteId)
            .Select(s => (SiteLifecycleStatus?)s.LifecycleStatus)
            .FirstOrDefaultAsync(ct);

        if (state is null || state == SiteLifecycleStatus.Closed)
        {
            return false;
        }

        return employee.Role == AppRoles.Admin ||
               (employee.Role == AppRoles.SiteManager &&
                IsAssignedToSite(employee, siteId));
    }

    public async Task<bool> CanViewArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct)
    {
        int? siteId = await GetArtifactSiteIdAsync(artifactId, ct);

        return siteId is int id &&
               await CanViewSiteAsync(principal, id, ct);
    }

    public async Task<bool> CanManageArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        if (employee is null)
        {
            return false;
        }

        var resource = await db.Artifacts
            .AsNoTracking()
            .Where(a => a.Id == artifactId)
            .Select(a => new
            {
                a.SiteId,
                a.CustodyStatus,
                SiteLifecycleStatus = a.Site!.LifecycleStatus
            })
            .FirstOrDefaultAsync(ct);

        if (resource is null ||
            resource.SiteLifecycleStatus == SiteLifecycleStatus.Closed ||
            resource.CustodyStatus is ArtifactCustodyStatus.InTransit
                or ArtifactCustodyStatus.Warehoused)
        {
            return false;
        }

        return employee.Role == AppRoles.Admin ||
               (employee.Role == AppRoles.SiteManager &&
                IsAssignedToSite(employee, resource.SiteId));
    }

    public async Task<bool> CanVerifyArtifactMediaAsync(
        ClaimsPrincipal principal,
        int mediaId,
        CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        if (employee is null)
        {
            return false;
        }

        int? siteId = await db.MediaFiles
            .AsNoTracking()
            .Where(m => m.Id == mediaId)
            .Select(m => (int?)m.Artifact!.SiteId)
            .FirstOrDefaultAsync(ct);

        return siteId is int id &&
               (employee.Role == AppRoles.Admin ||
                (employee.Role == AppRoles.SiteManager &&
                 IsAssignedToSite(employee, id)));
    }

    public async Task<bool> CanRequestSitePublicAccessAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        if (employee is null ||
            employee.Role != AppRoles.SiteManager ||
            !IsAssignedToSite(employee, siteId))
        {
            return false;
        }

        return await db.Sites.AsNoTracking().AnyAsync(
            s => s.Id == siteId &&
                 s.LifecycleStatus == SiteLifecycleStatus.Active &&
                 !s.IsPublic &&
                 s.VerificationStatus != VerificationStatus.Pending,
            ct);
    }

    public async Task<bool> CanRequestArtifactPublicAccessAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        if (employee is null || employee.Role != AppRoles.SiteManager)
        {
            return false;
        }

        return await db.Artifacts.AsNoTracking().AnyAsync(
            a => a.Id == artifactId &&
                 employee.AssignedSiteId == a.SiteId &&
                 a.Site!.LifecycleStatus == SiteLifecycleStatus.Active &&
                 a.CustodyStatus == ArtifactCustodyStatus.OnSite &&
                 !a.IsPublic &&
                 a.VerificationStatus != VerificationStatus.Pending,
            ct);
    }

    public async Task<bool> CanPublishSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct)
    {
        return await IsAdminAsync(principal, ct) &&
               await db.Sites.AsNoTracking().AnyAsync(s => s.Id == siteId, ct);
    }

    public async Task<bool> CanPublishArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct)
    {
        if (!await IsAdminAsync(principal, ct))
        {
            return false;
        }

        // Publishing the Artifact is allowed only when its parent Site is
        // already publicly eligible. The publication command must verify and
        // publish the Artifact in the same save operation.
        return await db.Artifacts.AsNoTracking().AnyAsync(
            a => a.Id == artifactId &&
                 a.Site!.IsPublic &&
                 a.Site.VerificationStatus == VerificationStatus.Verified,
            ct);
    }

    public async Task<bool> CanUnpublishSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct)
    {
        return await IsAdminAsync(principal, ct) &&
               await db.Sites.AsNoTracking().AnyAsync(
                   s => s.Id == siteId && s.IsPublic,
                   ct);
    }

    public async Task<bool> CanUnpublishArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct)
    {
        return await IsAdminAsync(principal, ct) &&
               await db.Artifacts.AsNoTracking().AnyAsync(
                   a => a.Id == artifactId && a.IsPublic,
                   ct);
    }

    private async Task<bool> IsAdminAsync(
        ClaimsPrincipal principal,
        CancellationToken ct)
    {
        CurrentEmployee? employee =
            await currentEmployeeService.GetAsync(principal, ct);

        return employee?.Role == AppRoles.Admin;
    }

    private Task<int?> GetArtifactSiteIdAsync(
        int artifactId,
        CancellationToken ct) =>
        db.Artifacts
            .AsNoTracking()
            .Where(a => a.Id == artifactId)
            .Select(a => (int?)a.SiteId)
            .FirstOrDefaultAsync(ct);

    private static bool HasOrganizationWideReadScope(
        CurrentEmployee employee) =>
        AppRoles.HasOrganizationWideReadScope(employee.Role);

    private static bool IsAssignedToSite(
        CurrentEmployee employee,
        int siteId) =>
        AppRoles.IsOnSiteRole(employee.Role) &&
        employee.AssignedSiteId == siteId;

}
