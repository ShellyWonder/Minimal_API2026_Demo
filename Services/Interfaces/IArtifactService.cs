namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface IArtifactService
    {
        #region Public Signatures
        public Task<List<PublicArtifactResponse>> GetAllPublicArtifactsAsync(CancellationToken ct);
        public Task<List<PublicArtifactResponse>?> GetPublicArtifactsBySiteAsync(int siteId, CancellationToken ct);
        #endregion

        #region Private Signatures
        public Task<List<PrivateArtifactResponse>> GetAllPrivateArtifactsAsync(CancellationToken ct);

        public Task<List<PrivateArtifactResponse>?> GetPrivateArtifactsBySiteAsync(int siteId, CancellationToken ct);


        #endregion
    }
}
