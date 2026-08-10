
namespace ThePlatoProject.Contracts.Requests
{
    public class CreateArtifactRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string CatalogNumber { get; set; } = string.Empty;

        [MaxLength(2500)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? PublicNarrative { get; set; }

        [Required]
        public DateTimeOffset DateDiscoveredUtc { get; set; } 
        [Required]
        public ArtifactType Type { get; set; } = ArtifactType.Unknown; //artifact type enum
       
        [Required]
        public int SiteId { get; set; }

        
    }
}
