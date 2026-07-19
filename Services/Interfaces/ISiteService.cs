namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface ISiteService
    {
        public Task<List<PublicSiteResponse>> GetAllSitesPublicAsync(CancellationToken ct);
    }
}
