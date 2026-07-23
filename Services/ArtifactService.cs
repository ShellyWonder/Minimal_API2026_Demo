namespace MinimalAPI2026Demo.Services
{
    public class ArtifactService(ApplicationDbContext db) : IArtifactService
    {

        public async Task<List<PublicArtifactResponse>> GetAllPublicArtifactsAsync(CancellationToken ct)
        {
            return await db.Artifacts
                           .AsNoTracking()
                           .Include(a => a.Site)
                           .Include(a => a.MediaFiles)
                           .Select(a => new PublicArtifactResponse
                           {
                               Id = a.Id,
                               Name = a.Name,
                               CatalogNumber = a.CatalogNumber ?? string.Empty,
                               PublicNarrative = a.PublicNarrative ?? string.Empty,
                               DateDiscovered = a.DateDiscovered,
                               Type = a.Type!.ToString(),
                               //navigation properties
                               SiteName = a.Site != null ? a.Site.Name! : string.Empty,
                               PrimaryImageUrl = a.MediaFiles
                                                .Where(m => m.IsPrimary)
                                                .Select(m => $"/api/public/artifacts/images/{m.Id}")
                                                .FirstOrDefault()
                           })
                
                            .ToListAsync(ct);
        }

        public async Task<List<PrivateArtifactResponse>> GetAllPrivateArtifactsAsync(CancellationToken ct)
        {
            return await db.Artifacts
                           .AsNoTracking()
                           .Include(a => a.Site)
                           .Include(a => a.MediaFiles)
                           .Select(a => new PrivateArtifactResponse
                           {
                               Id = a.Id,
                               Name = a.Name,
                               CatalogNumber = a.CatalogNumber ?? string.Empty,
                               PublicNarrative = a.PublicNarrative ?? string.Empty,
                               Description = a.Description ?? string.Empty,
                               DateDiscovered = a.DateDiscovered,
                               Type = a.Type!.ToString(),
                               //navigation properties
                               SiteName = a.Site != null ? a.Site.Name! : string.Empty,
                               PrimaryImageUrl = a.MediaFiles
                                                .Where(m => m.IsPrimary)
                                                .Select(m => $"/api/private/artifacts/images/{m.Id}")
                                                .FirstOrDefault()
                           })

                            .ToListAsync(ct);
        }

        public async Task<List<PublicArtifactResponse>?> GetPublicArtifactsBySiteAsync(int siteId, CancellationToken ct)
        {
            var siteExists = await db.Sites
                                     .AsNoTracking()
                                     .AnyAsync(s => s.Id == siteId, ct);
            if(!siteExists) return [];

            return await db.Artifacts
                           .AsNoTracking()
                           .Include(a => a.Site)
                           .Include(a => a.MediaFiles)
                           .Where(a => a.SiteId == siteId)
                           .Select(a => new PublicArtifactResponse
                           {
                               Id = a.Id,
                               Name = a.Name,
                               CatalogNumber = a.CatalogNumber ?? string.Empty,
                               PublicNarrative = a.PublicNarrative ?? string.Empty,
                               DateDiscovered = a.DateDiscovered,
                               Type = a.Type.ToString() ?? "unknown",
                               SiteName = a.Site != null ? a.Site.Name! : string.Empty,
                               PrimaryImageUrl = a.MediaFiles
                                    .Where(m => m.IsPrimary)
                                    .Select(m => $"/api/public/artifacts/images/{m.Id}")
                                    .FirstOrDefault()

                           }).ToListAsync(ct);

        }

        public async Task<List<PrivateArtifactResponse>?> GetPrivateArtifactsBySiteAsync(int siteId, CancellationToken ct)
        {
            var siteExists = await db.Sites
                                     .AsNoTracking()
                                     .AnyAsync(s => s.Id == siteId, ct);
            if (!siteExists) return [];

            return await db.Artifacts
                           .AsNoTracking()
                           .Include(a => a.Site)
                           .Include(a => a.MediaFiles)
                           .Where(a => a.SiteId == siteId)
                           .Select(a => new PrivateArtifactResponse 
                           {
                               Id = a.Id,
                               Name = a.Name,
                               CatalogNumber = a.CatalogNumber ?? string.Empty,
                               PublicNarrative = a.PublicNarrative ?? string.Empty,
                               Description = a.Description ?? string.Empty,
                               DateDiscovered = a.DateDiscovered,
                               Type = a.Type.ToString() ?? "unknown",
                               SiteName = a.Site != null ? a.Site.Name! : string.Empty,
                               PrimaryImageUrl = a.MediaFiles
                                    .Where(m => m.IsPrimary)
                                    .Select(m => $"/api/public/artifacts/images/{m.Id}")
                                    .FirstOrDefault()



                           }).ToListAsync(ct);
        }
    }
}
