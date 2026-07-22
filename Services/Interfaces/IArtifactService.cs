namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface IArtifactService
    {
        public Task<List<PublicArtifactResponse>> GetAllPublicArtifactsAsync(CancellationToken ct);
    }
}
