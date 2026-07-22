namespace MinimalAPI2026Demo.Models.Responses
{
    //performs the function of a DTO
    public class PublicArtifactResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CatalogNumber { get; set; } = string.Empty;
        public string PublicNarrative { get; set; } = string.Empty;
        public DateTime DateDiscovered { get; set; }
        public string Type { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
    }
}
