namespace MinimalAPI2026Demo.Services.Interfaces
{
    public interface ICatalogRecordsService
    {
        
        #region Get
        public Task<List<CatalogRecordResponse>>GetAllPrivateCatRecordsAsync(CancellationToken ct);
        public Task<List<CatalogRecordResponse>>GetAllPrivateCatRecordsByArtifactIdAsync(int artifactId,CancellationToken ct);
        public Task<CatalogRecordResponse?> GetCatalogRecordByIdAsync(int id, CancellationToken ct);
        #endregion

        #region Create | Update 
        public Task<CatalogRecordResponse?>CreateCatalogRecordAsync(string userId,CreateCatalogRecordRequest request, CancellationToken ct);
        //public Task<bool>UpdateCatalogRecordAsync(int id,CancellationToken ct);
        #endregion
    }
}
