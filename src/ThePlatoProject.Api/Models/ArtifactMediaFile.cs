namespace MinimalAPI2026Demo.Models
{
    public class ArtifactMediaFile
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "image/jpeg";
        public byte[] Data { get; set; } = []; //image stored as a byte array in db
        public bool IsPrimary { get; set; } = false; //mark image as primary (or "main") if more than one

        [Required]
        public string UploadedById { get; set; } = string.Empty;

        public DateTimeOffset UploadedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        public ApplicationUser UploadedBy { get; set; } = null!;

        //Verification properties
        public VerificationStatus VerificationStatus { get; set; }
                                    = VerificationStatus.Unverified;

        public DateTimeOffset? VerifiedAtUtc { get; set; }

        public string? VerifiedById { get; set; }

        public ApplicationUser? VerifiedBy { get; set; }

        //Archive properties
        public ArchiveWorkflowState ArchiveState { get; set; }
                                     = ArchiveWorkflowState.None;

        [MaxLength(1000)]
        public string? ArchiveReason { get; set; }

        public DateTimeOffset? ArchiveRequestedAtUtc { get; set; }

        public string? ArchiveRequestedById { get; set; }

        public DateTimeOffset? ArchiveAuthorizedAtUtc { get; set; }

        public string? ArchiveAuthorizedById { get; set; }

        public DateTimeOffset? ArchivedAtUtc { get; set; }

        public string? ArchivedById { get; set; }

        //Archive navigation properties
        public ApplicationUser? ArchiveRequestedBy { get; set; }

        public ApplicationUser? ArchiveAuthorizedBy { get; set; }

        public ApplicationUser? ArchivedBy { get; set; }


        public int ArtifactId { get; set; }//Foreign key
        public Artifact Artifact { get; set; } = null!; //navigation Property
    }
}