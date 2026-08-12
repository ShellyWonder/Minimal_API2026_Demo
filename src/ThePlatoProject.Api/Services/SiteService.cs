namespace MinimalAPI2026Demo.Services
{
    public class SiteService(ApplicationDbContext db) : ISiteService
    {
        #region Get <List> Sites
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

        public async Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct)
        {
            return await db.Sites
                .AsNoTracking()
                .Select(s => new PrivateSiteResponse
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Location = s.Location,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Description = s.Description,
                    PublicNarrative = s.PublicNarrative,
                    ALRECNarrative= s.ALRECNarrative
                })

                .ToListAsync(ct);
        }
        #endregion

        #region Get Site by Id
        public async Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int id, CancellationToken ct)
        {
            return await db.Sites
                           .AsNoTracking()
                           .Where(s => s.Id == id)
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
                           .FirstOrDefaultAsync(ct);
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
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Description = s.Description,
                    PublicNarrative = s.PublicNarrative,
                    ALRECNarrative= s.ALRECNarrative
                })
                .FirstOrDefaultAsync(ct);
        }
        #endregion

        #region Create | Update 
        //Note: Per SRS sites are closed, not deleted.
        //Therefore, there is no need to implement a delete method for sites. Instead, we implement a method to close a site.
        public async Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct)
        {
            var site = new Site
            {
                Name = request.Name,
                Location = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Description = request.Description ?? "No description available.",
               PublicNarrative = request.PublicNarrative,
                ALRECNarrative= request.ALRECNarrative ?? "No information available."
            };
            db.Sites.Add(site);
            await db.SaveChangesAsync(ct);

            return new PrivateSiteResponse
            {
                Id = site.Id,
                Name = site.Name,
                Location = site.Location,
                Latitude = site.Latitude,
                Longitude = site.Longitude,
                Description = site.Description,
                PublicNarrative = site.PublicNarrative,
                ALRECNarrative= site.ALRECNarrative
            };
        }

        public async Task<bool> UpdateSiteAsync(int id, UpdateSiteRequest request, CancellationToken ct)
        {
            var site = await db.Sites.FindAsync(id, ct);
            if (site is null) return false;

            site.Name = request.Name;
            site.Location = request.Location;
            site.Latitude = request.Latitude;
            site.Longitude = request.Longitude;
            site.Description = request.Description ?? "No description available.";
            site.PublicNarrative = request.PublicNarrative;
            site.ALRECNarrative= request.ALRECNarrative ?? "No information available.";

            await db.SaveChangesAsync(ct);

            return true;

        }

        #endregion
    }
}
