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
                               SiteName = a.Site != null ? a.Site.Name! : string.Empty,
                               PrimaryImageUrl = a.MediaFiles
                    .Where(m => m.IsPrimary)
                    .Select(m => $"/api/public/artifacts/images/{m.Id}")
                    .FirstOrDefault()
                           })
                
                .ToListAsync(ct);
        }
    }
}
