namespace MinimalAPI2026Demo.Models.Requests
{
    public class CreateCatalogRecordRequest
    {
        public int ArtifactId { get; set; }
        public string Status { get; set; } = "New";
    }
}