namespace MinimalAPI2026Demo.Services
{
    public class ArtifactService(ApplicationDbContext db) : IArtifactService
    {
        #region Get <List> Artifacts
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
        #endregion

        #region Get Artifacts by Site
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
                               Type = a.Type!.ToString() ?? "unknown",
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
                               Type = a.Type!.ToString() ?? "unknown",
                               SiteName = a.Site != null ? a.Site.Name! : string.Empty,
                               PrimaryImageUrl = a.MediaFiles
                                    .Where(m => m.IsPrimary)
                                    .Select(m => $"/api/public/artifacts/images/{m.Id}")
                                    .FirstOrDefault()



                           }).ToListAsync(ct);
        }

        public async Task<PrivateArtifactResponse?> CreateArtifactAsync(CreateArtifactRequest request, CancellationToken ct)
        {
            //validate site 
            var site = await db.Sites.FindAsync(request.SiteId,ct);
            if(site == null) return null;// response 404NotFound

            // Validate the artifact type string
            if (!Enum.TryParse<ArtifactType>(request.Type, true, out var artifactType))
            {
                throw new ArgumentException($"Invalid artifact type '{request.Type}'. " +
                    $"Allowed values are: {string.Join(", ", Enum.GetNames(typeof(ArtifactType)))}");
            }

            //Create new artifact
            var artifact = new Artifact
            {
                Name = request.Name,
                CatalogNumber = request.CatalogNumber ?? string.Empty,
                Description = request.Description ?? string.Empty,
                PublicNarrative = request.PublicNarrative ?? string.Empty,
                DateDiscovered = request.DateDiscovered,
                Type = artifactType.ToString() ?? "unknown",
                SiteId = request.SiteId
            };

            db.Artifacts.Add(artifact);
            await db.SaveChangesAsync(ct);
            
            // return DTO
            return new PrivateArtifactResponse
            {
                Id = artifact.Id,
                Name = artifact.Name,
                CatalogNumber = artifact.CatalogNumber,
                Description = artifact.Description,
                PublicNarrative = artifact.PublicNarrative,
                DateDiscovered = artifact.DateDiscovered,
                Type = artifact.Type.ToString(),
                SiteId = artifact.SiteId,
                SiteName = site.Name ?? string.Empty,
                PrimaryImageUrl = ""
            };
        }

        public async Task<PublicArtifactResponse?> GetPublicArtifactByIdAsync(int Id, CancellationToken ct)
        {
            return await db.Artifacts
                           .AsNoTracking()
                           .Where(a => a.Id == Id)
                           .Include(a => a.Site)
                           .Include(a => a.MediaFiles)
                           .Select(a => new PublicArtifactResponse
                           {
                               Id = a.Id,
                               Name = a.Name,
                               CatalogNumber = a.CatalogNumber ?? string.Empty,
                               PublicNarrative = a.PublicNarrative ?? string.Empty,
                               DateDiscovered = a.DateDiscovered,
                               Type = a.Type!.ToString() ?? "unknown",
                               SiteName = a.Site != null ? a.Site.Name! : "unknown",
                               PrimaryImageUrl = a.MediaFiles
                                    .Where(m => m.IsPrimary)
                                    .Select(m => $"/api/public/artifacts/images/{m.Id}")
                                    .FirstOrDefault()
                           }) 
                           .FirstOrDefaultAsync(ct);
        }

        public async Task<PrivateArtifactResponse?> GetPrivateArtifactByIdAsync(int Id, CancellationToken ct)
        {
             return await db.Artifacts
                           .AsNoTracking()
                           .Where(a => a.Id == Id)
                           .Include(a => a.Site)
                           .Include(a => a.MediaFiles)
                           .Select(a => new PrivateArtifactResponse
                           {
                               Id = a.Id,
                               Name = a.Name,
                               CatalogNumber = a.CatalogNumber ?? string.Empty,
                               PublicNarrative = a.PublicNarrative ?? string.Empty,
                               DateDiscovered = a.DateDiscovered,
                               Type = a.Type!.ToString() ?? "unknown",
                               SiteName = a.Site != null ? a.Site.Name! : "unknown",
                               PrimaryImageUrl = a.MediaFiles
                                    .Where(m => m.IsPrimary)
                                    .Select(m => $"/api/private/artifacts/images/{m.Id}")
                                    .FirstOrDefault()
                           })
                           .FirstOrDefaultAsync(ct);
        }
        #endregion
    }
}
