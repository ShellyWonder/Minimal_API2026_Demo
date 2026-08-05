namespace MinimalAPI2026Demo.Models
{
    public class CatalogRecord
    {
        // All artifact children are archived with the artifact, so no need for a separate archival workflow here.
        // The artifact's archival status will be used to determine if the catalog record is archived or not.

        public int Id { get; set; }
        [Required]
        public int ArtifactId { get; set; }
        public Artifact Artifact { get; set; } = null!;
        [Required]
        public string SubmittedById { get; set; } = string.Empty;
        public ApplicationUser SubmittedBy { get; set; } = null!;

        [Required]
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
        public ICollection<CatalogNote> Notes { get; set; } = [];

        //Verification properties
        public string? VerifiedById { get; set; }
        public ApplicationUser? VerifiedBy { get; set; }

        public VerificationStatus VerificationStatus { get; set; }
                                               = VerificationStatus.Unverified;
        public DateTimeOffset? VerifiedAtUtc { get; set; }




    }
}
