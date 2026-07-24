namespace MinimalAPI2026Demo.Models.Requests
{
    public class UpdateArtifactRequest
    {
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

        [Required]
        public int SiteId { get; set; }

    }
}