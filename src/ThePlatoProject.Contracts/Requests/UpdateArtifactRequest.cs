namespace ThePlatoProject.Contracts.Requests
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

        public DateTimeOffset DateDiscoveredUtc { get; set; } 

        public ArtifactType Type { get; set; } //artifact type

        [Required]
        public int SiteId { get; set; }

    }
}