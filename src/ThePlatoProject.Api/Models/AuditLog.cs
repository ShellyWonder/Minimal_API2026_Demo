namespace MinimalAPI2026Demo.Models
{
    public class AuditLog
    {
        public long Id { get; set; }

        public DateTimeOffset OccurredAtUtc { get; set; }
            = DateTimeOffset.UtcNow;

        public string? ActorUserId { get; set; }

        [MaxLength(250)]
        public string? ActorDisplayName { get; set; }

        [Required, MaxLength(150)]
        public string Action { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string EntityType { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string EntityId { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Summary { get; set; }

        public string? ChangesJson { get; set; }

        [MaxLength(1000)]
        public string? Reason { get; set; }

        [MaxLength(100)]
        public string? CorrelationId { get; set; }

        public ApplicationUser? ActorUser { get; set; }
    }
}
