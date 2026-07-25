namespace MinimalAPI2026Demo.Services
{
    public class CatalogRecordsService : ICatalogRecordsService
    {
        #region Get
        public Task<List<PrivateCatalogRecordResponse>> GetAllPrivateCatRecordsAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<PrivateCatalogRecordResponse>> GetAllPrivateCatRecordsByArtifactAsync(int artifactId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<PublicCatalogRecordResponse>> GetAllPublicCatRecordsByArtifactAsync(int artifactId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<PrivateCatalogRecordResponse> GetPrivateCatalogRecordByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<PublicCatalogRecordResponse> GetPublicCatalogRecordByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Create | Update | Delete

        public Task<PrivateCatalogRecordResponse?> CreateCatalogRecordAsync(CreateCatalogRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCatalogRecordAsync(int recordId, UpdateCatalogRecordRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCatalogRecordAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
