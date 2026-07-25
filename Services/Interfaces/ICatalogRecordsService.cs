namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface ICatalogRecordsService
    {
        #region Public Signatures
        public Task<List<PublicCatalogRecordResponse>>GetAllPublicCatRecordsByArtifactAsync(int artifactId,CancellationToken ct);
        public Task<PublicCatalogRecordResponse> GetPublicCatalogRecordByIdAsync(int id, CancellationToken ct);

        #endregion

        #region Private Signatures
        public Task<List<PrivateCatalogRecordResponse>>GetAllPrivateCatRecordsAsync(CancellationToken ct);
        public Task<List<PrivateCatalogRecordResponse>>GetAllPrivateCatRecordsByArtifactAsync(int artifactId,CancellationToken ct);
        public Task<PrivateCatalogRecordResponse> GetPrivateCatalogRecordByIdAsync(int id, CancellationToken ct);

        #region Create | Update | Delete
        public Task<PrivateCatalogRecordResponse?>CreateCatalogRecordAsync(CreateCatalogRequest request, CancellationToken ct);
        public Task<bool>UpdateCatalogRecordAsync(int recordId,UpdateCatalogRecordRequest request, CancellationToken ct);
        public Task<bool>DeleteCatalogRecordAsync(int id, CancellationToken ct);
        #endregion
        #endregion
    }
}
