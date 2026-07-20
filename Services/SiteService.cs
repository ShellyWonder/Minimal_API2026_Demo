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
                               Cooridinates = s.Coordinates,
                               Latitude = s.Latitude,
                               Longitude = s.Longitude,
                               Description = s.Description,
                               PublicNarrative = s.PublicNarrative
                           })
                           .ToListAsync(ct);
        }

        public async Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int id, CancellationToken ct)
        {
            return await db.Sites
                           .AsNoTracking()
                           .Where(s => s.Id == id)
                           .Select(s => new PublicSiteResponse
                           {
                               Id = s.Id,
                               Name = s.Name!,
                               Cooridinates = s.Coordinates,
                               Location = s.Location,
                               Latitude = s.Latitude,
                               Longitude = s.Longitude,
                               Description = s.Description,
                               PublicNarrative = s.PublicNarrative
                           })
                           .FirstOrDefaultAsync(ct);
        }

        public async Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct)
        {
            return await db.Sites
                .AsNoTracking()
                .Select(s => new PrivateSiteResponse
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Location = s.Location,
                    Cooridinates = s.Coordinates,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Description = s.Description,
                    PublicNarrative = s.PublicNarrative,
                    AeonNarrative = s.AeonNarrative
                })
                
                .ToListAsync(ct);
        }


        public async Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct)
        {
            return await db.Sites
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new PrivateSiteResponse
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Location = s.Location,
                    Cooridinates = s.Coordinates,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Description = s.Description,
                    PublicNarrative = s.PublicNarrative,
                    AeonNarrative = s.AeonNarrative
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
