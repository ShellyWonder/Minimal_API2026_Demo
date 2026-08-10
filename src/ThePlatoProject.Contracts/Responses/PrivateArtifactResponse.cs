namespace ThePlatoProject.Contracts.Responses
{
    public class PrivateArtifactResponse
    {
        // Descriptive properties
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CatalogNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PublicNarrative { get; set; } = string.Empty;
        public DateTimeOffset DateDiscoveredUtc { get; set; }
        public string Type { get; set; } = string.Empty;

        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }

        // Parent state needed by the Artifact UI
        public SiteLifecycleStatus SiteLifecycleStatus { get; set; }

        // Visibility
        public bool IsPublic { get; set; }

        // Verification
        public VerificationStatus VerificationStatus { get; set; }
        public DateTimeOffset? VerifiedAtUtc { get; set; }
        public string? VerifiedBy { get; set; }

        // Custody lifecycle
        public ArtifactCustodyStatus CustodyStatus { get; set; }
        public string? TransferReason { get; set; }

        public DateTimeOffset? TransferRequestedAtUtc { get; set; }
        public string? TransferRequestedBy { get; set; }

        public DateTimeOffset? TransferAuthorizedAtUtc { get; set; }
        public string? TransferAuthorizedBy { get; set; }

        public DateTimeOffset? ShipmentSentAtUtc { get; set; }
        public string? ShipmentSentBy { get; set; }

        public DateTimeOffset? ShipmentReceivedAtUtc { get; set; }
        public string? ShipmentReceivedBy { get; set; }

        public string? WarehouseLocation { get; set; }

        public DateTimeOffset? TransferCompletedAtUtc { get; set; }
        public string? TransferCompletedBy { get; set; }
    }
}
