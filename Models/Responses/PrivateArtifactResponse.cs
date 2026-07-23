namespace MinimalAPI2026Demo.Models.Responses
{
    public class PrivateArtifactResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CatalogNumber { get; set; } = string.Empty;
        public string PublicNarrative { get; set; } = string.Empty;
        public DateTime DateDiscovered { get; set; }
        public string Type { get; set; } = string.Empty;

        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
        public string? Description { get; set; } // Internal research description

    }
}
