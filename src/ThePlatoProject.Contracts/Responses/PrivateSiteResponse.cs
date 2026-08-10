namespace ThePlatoProject.Contracts.Responses
{
    public class PrivateSiteResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Coordinates { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public string? PublicNarrative { get; set; }
        public string? ALRECNarrative{ get; set; }

        //Visibility 
        public bool IsPublic { get; set; }

        //Verification
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
        public DateTimeOffset VerifiedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public string VerifiedBy { get; set; } = string.Empty;

        //Site lifecycle workflow properties
        public SiteLifecycleStatus LifecycleStatus { get; set; } = SiteLifecycleStatus.Active;

        [Required, MaxLength(2500)]
        public string? ClosureReason { get; set; }
        public DateTimeOffset? ClosureRequestedAtUtc { get; set; }
        public string? ClosureRequestedById { get; set; }
        public DateTimeOffset? ClosureAuthorizedAtUtc { get; set; }
        public string? ClosureAuthorizedById { get; set; }
        [Required]
        public DateTimeOffset? ClosedAtUtc { get; set; }
        [Required]
        public string ClosedById { get; set; } = string.Empty;

    }
}
