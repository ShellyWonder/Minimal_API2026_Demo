namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface ISiteService
    {
        public Task<List<PublicSiteResponse>> GetAllSitesPublicAsync(CancellationToken ct);
        public Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int id, CancellationToken ct);
        public Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct);
        public Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct);
        public Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct);
        public Task<bool> UpdateSiteAsync(int id, UpdateSiteRequest request, CancellationToken ct);
        public Task<bool> DeleteSiteAsync(int id, CancellationToken ct);
    }
}
