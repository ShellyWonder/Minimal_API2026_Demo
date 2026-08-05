namespace MinimalAPI2026Demo.Models
{
    public class Site
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string? Name { get; set; }

        [Required, MaxLength(100)]
        public string? Location { get; set; }

        [MaxLength(100)]
        public string? Coordinates { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [Required, MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? PublicNarrative { get; set; }

        [MaxLength(2000)]
        public string? ALRECNarrative{ get; set; }

        public bool IsPublic { get; set; }

        //Verify public or private site status
        public VerificationStatus VerificationStatus { get; set; }
                                           = VerificationStatus.Unverified;

        public DateTimeOffset? VerifiedAtUtc { get; set; }

        public string? VerifiedById { get; set; }

        // Verification navigation
        public ApplicationUser? VerifiedBy { get; set; }

        //archive process workflow
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

        //archive navigation properties
        public ApplicationUser? ArchiveRequestedBy { get; set; }

        public ApplicationUser? ArchiveAuthorizedBy { get; set; }

        public ApplicationUser? ArchivedBy { get; set; }

        //collection navigation properties
        public ICollection<Artifact> Artifacts { get; set; } = [];

        public ICollection<ApplicationUser> AssignedEmployees { get; set; } = [];
    }
}
