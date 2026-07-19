namespace MinimalAPI2026Demo.Services
{
    public class SiteService(ApplicationDbContext db) : ISiteService
    {
        public async Task<List<PublicSiteResponse>> GetAllSitesPublicAsync(CancellationToken ct)
        {
            return await db.Sites
                           .AsNoTracking()
                           .Select(s => new PublicSiteResponse
                           {
                               Id = s.Id,
                               Name = s.Name!,
                               Location = s.Location,
                               Latitude = s.Latitude,
                               Longitude = s.Longitude,
                               Description = s.Description,
                               PublicNarrative = s.PublicNarrative
                           })
                           .ToListAsync(ct);
        }
    }
}
