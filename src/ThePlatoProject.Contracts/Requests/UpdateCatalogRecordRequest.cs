namespace ThePlatoProject.Contracts.Requests
{
    public class UpdateCatalogRecordRequest
    {
        public string? VerifiedById { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
