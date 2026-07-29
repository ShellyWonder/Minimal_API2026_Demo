namespace MinimalAPI2026Demo.Services
{
    public class ArtifactMediaFileService(ApplicationDbContext db) : IArtifactMediaFileService
    {
        public async Task<ArtifactMediaFile?> CreateArtifactMediaFileAsync(int artifactId, 
                                                                     IFormFile file, 
                                                                     bool isPrimary,
                                                                     CancellationToken ct)
        {
            //validate artifact
            var  artifact = await db.Artifacts.FindAsync([artifactId], ct);
            if (artifact is null) return null;

            //validate file input
            if (file is null || file.Length == 0) throw new ArgumentException("File cannot be empty.");

            // If this file is primary, clear any existing primary images
            if(isPrimary)
            {
                var existingPrimary = await db.MediaFiles
                                              .Where(m => m.Id == artifactId && m.IsPrimary == true)
                                              .ToListAsync(ct);
                foreach (var media in existingPrimary) 
                                        media.IsPrimary = false;
            }
            // Convert IFormFile to byte array
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var data = ms.ToArray();

            // Create new media record
            var mediaFile = new ArtifactMediaFile
            {
                ArtifactId = artifactId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = data,
                IsPrimary = isPrimary
            };

            db.MediaFiles.Add(mediaFile);
            await db.SaveChangesAsync(ct);
            return mediaFile;
        }
    }
}
