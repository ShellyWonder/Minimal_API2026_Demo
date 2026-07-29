namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface IArtifactMediaFileService
    {
        public Task<ArtifactMediaFile?> CreateArtifactMediaFileAsync(int artifactId,
                                                                     IFormFile file,
                                                                     bool isPrimary,
                                                                     CancellationToken ct);

    }
}
