namespace MinimalAPI2026Demo.Models.Requests
{
    public class CreateArtifactRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string CatalogNumber { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? PublicNarrative { get; set; }

        [Required]
        public DateTime DateDiscovered { get; set; } = DateTime.UtcNow;

        [Required]
        public string Type { get; set; } = string.Empty; //artifact type enum as a string
       
        [Required]
        public int SiteId { get; set; }

        
    }
}
