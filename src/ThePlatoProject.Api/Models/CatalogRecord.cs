namespace MinimalAPI2026Demo.Models
{
    public class CatalogRecord
    {
        // Catalog Records remain attached to the Artifact lifecycle.
        // They cannot be archived independently.
        public int Id { get; set; }
        [Required]
        public int ArtifactId { get; set; }
        public Artifact Artifact { get; set; } = null!;
        [Required]
        public string SubmittedById { get; set; } = string.Empty;
        public ApplicationUser SubmittedBy { get; set; } = null!;

        [Required]
        public DateTimeOffset DateSubmittedUtc { get; set; } = DateTimeOffset.UtcNow;
        public ICollection<CatalogRecordNote> Notes { get; set; } = [];



    }
}
