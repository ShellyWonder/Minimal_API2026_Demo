using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPI2026Demo.Models
{
    public class Site
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? PublicNarrative { get; set; }

        [MaxLength(2000)]
        public string ALRECNarrative { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        //Verify public or private site status
        public VerificationStatus VerificationStatus { get; set; }
                                           = VerificationStatus.Unverified;

        public DateTimeOffset? VerifiedAtUtc { get; set; }

        public string? VerifiedById { get; set; }

        // Verification navigation
        public ApplicationUser? VerifiedBy { get; set; }

        //site closure workflow properties
        public SiteLifecycleStatus LifecycleStatus { get; set; }
                                = SiteLifecycleStatus.Active;

        [MaxLength(1000)]
        public string? ClosureReason { get; set; }

        public DateTimeOffset? ClosureRequestedAtUtc { get; set; }
        public string? ClosureRequestedById { get; set; }

        public DateTimeOffset? ClosureAuthorizedAtUtc { get; set; }
        public string? ClosureAuthorizedById { get; set; }

        public DateTimeOffset? ClosedAtUtc { get; set; }
        public string? ClosedById { get; set; }

        //collection navigation properties
        public ICollection<Artifact> Artifacts { get; set; } = [];

        public ICollection<ApplicationUser> AssignedEmployees { get; set; } = [];

        //closure navigation properties
        public ApplicationUser? ClosureRequestedBy { get; set; }
        public ApplicationUser? ClosureAuthorizedBy { get; set; }
        public ApplicationUser? ClosedBy { get; set; }
    }
}
