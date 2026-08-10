namespace ThePlatoProject.Contracts.Responses
{
    public class PublicArtifactResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CatalogNumber { get; set; } = string.Empty;
        public string PublicNarrative { get; set; } = string.Empty;
        public DateTimeOffset DateDiscoveredUtc { get; set; } 
        public string Type { get; set; } = string.Empty;

        public string SiteName { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
    }
}
