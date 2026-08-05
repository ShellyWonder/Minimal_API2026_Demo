namespace MinimalAPI2026Demo.Models
{
    public class DirectMessage
    {
        public long Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string RecipientId { get; set; } = string.Empty;

        [Required, MaxLength(5000)]
        public string Body { get; set; } = string.Empty;

        public DateTimeOffset SentAtUtc { get; set; }
            = DateTimeOffset.UtcNow;

        public bool IsRead { get; set; }

        public DateTimeOffset? ReadAtUtc { get; set; }

        public bool IsDeletedBySender { get; set; }

        public DateTimeOffset? DeletedBySenderAtUtc { get; set; }

        public bool IsDeletedByRecipient { get; set; }

        public DateTimeOffset? DeletedByRecipientAtUtc { get; set; }

        public ApplicationUser Sender { get; set; } = null!;

        public ApplicationUser Recipient { get; set; } = null!;
    }
}