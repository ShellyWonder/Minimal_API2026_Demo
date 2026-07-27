namespace MinimalAPI2026Demo.Services
{
    public class CatalogRecordsService(ApplicationDbContext db) : ICatalogRecordsService
    {
        #region Get
        public Task<List<CatalogRecordResponse>> GetAllPrivateCatRecordsAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CatalogRecordResponse>> GetAllPrivateCatRecordsByArtifactIdAsync(int artifactId, CancellationToken ct)
        {
            //verify the artifact
            var artifactExists = await db.Artifacts
                                          .AsNoTracking()
                                          .AnyAsync(a => a.Id == artifactId);
            if (!artifactExists) return null!;

            return await db.CatalogRecords
                           .AsNoTracking()
                           .Where(cr => cr.ArtifactId == artifactId)
                           .Include(cr => cr.VerifiedBy)
                           .Include(cr => cr.SubmittedBy)
                           .Include(cr => cr.Notes)
                             .ThenInclude(n => n.Author)
                           .Select(cr => new CatalogRecordResponse
                           {
                               Id = cr.Id,
                               ArtifactId = cr.ArtifactId,
                               Status = cr.Status,
                               DateSubmitted = cr.DateSubmitted,
                               SubmittedBy = $"{cr.SubmittedBy.FirstName} {cr.SubmittedBy.LastName}",
                               VerifiedBy = cr.VerifiedBy != null
                                                        ? $"{cr.VerifiedBy.FirstName} {cr.VerifiedBy.LastName}"
                                                        : null,
                               Notes = cr.Notes.Select(n => new CatalogNoteResponse
                               {
                                   Id = n.Id,
                                   Content = n.Content,
                                   Created = n.Created,
                                   Author = $"{n.Author.FirstName} {n.Author.LastName}"

                               }).ToList()

                           }).ToListAsync(ct);
        }

        public async Task<CatalogRecordResponse?> GetCatalogRecordByIdAsync(int id, CancellationToken ct)
        {
            var recordExists = await db.CatalogRecords.AnyAsync(cr => cr.Id == id);
            if (!recordExists) return null!;

            return await db.CatalogRecords
                           .AsNoTracking()
                           .Where(cr => cr.Id == id)
                           .Include(cr => cr.VerifiedBy)
                           .Include(cr => cr.SubmittedBy)
                           .Include(cr => cr.Notes)
                             .ThenInclude(n => n.Author)
                           .Select(cr => new CatalogRecordResponse
                           {
                               Id = cr.Id,
                               ArtifactId = cr.ArtifactId,
                               Status = cr.Status,
                               DateSubmitted = cr.DateSubmitted,
                               SubmittedBy = $"{cr.SubmittedBy.FirstName} {cr.SubmittedBy.LastName}" ?? string.Empty,
                               VerifiedBy = cr.VerifiedBy != null
                                                        ? $"{cr.VerifiedBy.FirstName} {cr.VerifiedBy.LastName}"
                                                        : null,
                               Notes = cr.Notes.Select(n => new CatalogNoteResponse
                               {
                                   Id = n.Id,
                                   Content = n.Content,
                                   Created = n.Created,
                                   Author = $"{n.Author.FirstName} {n.Author.LastName}"

                               }).ToList()
                           }).FirstOrDefaultAsync(ct);
        }


        #endregion

        #region Create | Update | Delete

        public async Task<CatalogRecordResponse?> CreateCatalogRecordAsync(string userId,
                                                                            CreateCatalogRecordRequest request,
                                                                            CancellationToken ct)
        {
            var artifact = await db.Artifacts
               .AsNoTracking()
               .FirstOrDefaultAsync(a => a.Id == request.ArtifactId, ct);
            if (artifact is null) return null; //response BadRequest

            var record = new CatalogRecord
            {
                ArtifactId = request.ArtifactId,
                Status = request.Status,
                SubmittedById = userId,
                DateSubmitted = DateTime.UtcNow,
            };
            db.CatalogRecords.Add(record);
            await db.SaveChangesAsync(ct);

            //Re-load with nav properties
            var created = await db.CatalogRecords
            .AsNoTracking()
            .Include(r => r.SubmittedBy)
            .FirstAsync(r => r.Id == record.Id, ct);

            //return DTO
            return new CatalogRecordResponse
            {
                Id = created.Id,
                ArtifactId = created.ArtifactId,
                Status = created.Status,
                DateSubmitted = created.DateSubmitted,
                SubmittedBy = $"{created.SubmittedBy.FirstName} {created.SubmittedBy.LastName}",
                VerifiedBy = null,
                Notes = new List<CatalogNoteResponse>()
            };
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
