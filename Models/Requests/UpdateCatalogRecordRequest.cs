namespace MinimalAPI2026Demo.Models.Requests
{
    public class UpdateCatalogRecordRequest
    {
        public string? VerifiedById { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
