namespace MinimalAPI2026Demo.Models
{
    public class CatalogRecord
    {
        public int Id { get; set; }
        [Required]
        public int ArtifactId { get; set; }
        public Artifact Artifact { get; set; } = null!;
        [Required]
        public string SubmittedById { get; set; } = string.Empty;
        public ApplicationUser SubmittedBy { get; set; } = null!;
        public string? VerifiedById { get; set; }
        public ApplicationUser? VerifiedBy { get; set; }

        [Required]
        public string Status { get; set; } = "Draft";
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
        public ICollection<CatalogNote> Notes { get; set; } = [];


    }
}
