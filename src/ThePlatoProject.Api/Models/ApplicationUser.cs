
namespace MinimalAPI2026Demo.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public bool IsActive { get; set; } = true;

        public DateTimeOffset? DeactivatedAtUtc { get; set; }

        public string? DeactivatedById { get; set; }

        [MaxLength(500)]
        public string? DeactivationReason { get; set; }

        public DateTimeOffset? RestoredAtUtc { get; set; }

        public string? RestoredById { get; set; }

        public int? AssignedSiteId { get; set; }

        // Reference navigation properties

        public ApplicationUser? DeactivatedBy { get; set; }

        public ApplicationUser? RestoredBy { get; set; }

        public Site? AssignedSite { get; set; }

        // Collection navigation properties
        public ICollection<CatalogRecord> SubmittedCatalogRecords { get; set; } = [];
        public ICollection<ArtifactMediaFile> UploadedMedia { get; set; } = [];

        public ICollection<CatalogRecordNote> AuthoredCatalogNotes { get; set; } = [];

        public ICollection<DirectMessage> SentMessages { get; set; } = [];

        public ICollection<DirectMessage> ReceivedMessages { get; set; } = [];

        public ICollection<AuditLog> AuditEvents { get; set; } = [];


    }

}
