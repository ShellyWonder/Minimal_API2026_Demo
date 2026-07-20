namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface ISiteService
    {
        public Task<List<PublicSiteResponse>> GetAllSitesPublicAsync(CancellationToken ct);
        public Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int id, CancellationToken ct);
        public Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct);
        public Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct);
    }
}
