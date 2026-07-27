namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface ICatalogRecordsService
    {
        
        #region Get
        public Task<List<CatalogRecordResponse>>GetAllPrivateCatRecordsAsync(CancellationToken ct);
        public Task<List<CatalogRecordResponse>>GetAllPrivateCatRecordsByArtifactIdAsync(int artifactId,CancellationToken ct);
        public Task<CatalogRecordResponse> GetPrivateCatalogRecordByIdAsync(int id, CancellationToken ct);

        #region Create | Update | Delete
        public Task<CatalogRecordResponse?>CreateCatalogRecordAsync(CreateCatalogRequest request, CancellationToken ct);
        public Task<bool>UpdateCatalogRecordAsync(int recordId,UpdateCatalogRecordRequest request, CancellationToken ct);
        public Task<bool>DeleteCatalogRecordAsync(int id, CancellationToken ct);
        #endregion
        #endregion
    }
}
