namespace MinimalAPI2026Demo.Authorization;

public static class AuthorizedQueryExtensions
{
    public static IQueryable<Site> VisibleSites(this DbSet<Site> sites,
                                                 CurrentEmployee employee)
    {
        IQueryable<Site> query = sites.AsNoTracking();

        if (AppRoles.HasOrganizationWideReadScope(employee.Role))
        {
            return query;
        }

        if (AppRoles.IsOnSiteRole(employee.Role) &&
            employee.AssignedSiteId is int siteId)
        {
            return query.Where(site => site.Id == siteId);
        }

        return query.Where(_ => false);
    }

    public static IQueryable<Artifact> VisibleArtifacts(
        this DbSet<Artifact> artifacts,
        CurrentEmployee employee)
    {
        IQueryable<Artifact> query = artifacts.AsNoTracking();

        if (AppRoles.HasOrganizationWideReadScope(employee.Role))
        {
            return query;
        }

        if (AppRoles.IsOnSiteRole(employee.Role) &&
            employee.AssignedSiteId is int siteId)
        {
            return query.Where(artifact => artifact.SiteId == siteId);
        }

        return query.Where(_ => false);
    }
}
