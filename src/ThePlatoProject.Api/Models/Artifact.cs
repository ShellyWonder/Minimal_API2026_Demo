namespace MinimalAPI2026Demo.Models
{
    public class Artifact
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string? CatalogNumber { get; set; }

        [MaxLength(2500)]
        public string? Description { get; set; }

        [MaxLength(2500)]
        public string? PublicNarrative { get; set; }

        public DateTime DateDiscovered { get; set; } = DateTime.UtcNow;

        public string? Type { get; set; } //artifact type

        public bool IsPublic { get; set; }

        //Verification and approval properties
        public VerificationStatus VerificationStatus { get; set; }
                                        = VerificationStatus.Unverified;

        public DateTimeOffset? VerifiedAtUtc { get; set; }

        public string? VerifiedById { get; set; }

        public ApplicationUser? VerifiedBy { get; set; }

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

        [Required]
        public int SiteId { get; set; } //foreign key
        public Site? Site { get; set; } //Nav property

        //archive navigation properties
        public ApplicationUser? ArchiveRequestedBy { get; set; }

        public ApplicationUser? ArchiveAuthorizedBy { get; set; }

        public ApplicationUser? ArchivedBy { get; set; }

        //Nav properties 
        public List<ArtifactMediaFile> MediaFiles { get; set; } = [];
        public List<CatalogRecord> CatalogRecords { get; set; } = [];

        //Custody properties tracking transfers of the artifact
        public DateTimeOffset? ShipmentSentAtUtc { get; set; }

        public string? ShipmentSentById { get; set; }

        public DateTimeOffset? ShipmentReceivedAtUtc { get; set; }

        public string? ShipmentReceivedById { get; set; }

        [MaxLength(250)]
        public string? WarehouseLocation { get; set; }

        //Nav properties for custody tracking
        public ApplicationUser? ShipmentSentBy { get; set; }

        public ApplicationUser? ShipmentReceivedBy { get; set; }
    }
}
