namespace MinimalAPI2026Demo.Models
{
    // All artifact children are archived with the artifact, so no need for a separate archival workflow here.
    // The artifact's archival status will be used to determine if the media file is archived or not.
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

        public int ArtifactId { get; set; }//Foreign key
        public Artifact Artifact { get; set; } = null!; //navigation Property
    }
}