namespace MinimalAPI2026Demo.Models
{
    public class Artifact
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string? Name { get; set; }

        [Required, MaxLength(500)]
        public string? Catalog { get; set; }

        [MaxLength(2500)]
        public string? Description { get; set; }

        [MaxLength(2500)]
        public string? PublicNarrative { get; set; }

        public DateTimeOffset DateDiscovered { get; set; } = DateTimeOffset.UtcNow;

        public string? Type { get; set; } //artifact type


        [Required]
        public int SiteId { get; set; } //foreign key
        public Site? Site { get; set; } //Nav property

        //Nav properties 
        public List<ArtifactMediaFile> MediaFiles { get; set; } = [new()];
        public List<CatalogRecord> CatalogRecords { get; set; } = [new()];
    }
}
