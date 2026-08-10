namespace ThePlatoProject.Contracts.Responses
{
    public class CatalogRecordResponse
    {
        public int Id { get; set; }
        [Required]
        public int ArtifactId { get; set; }

        [Required]
        public string SubmittedBy { get; set; } = string.Empty;
        [Required]
        public DateTimeOffset DateSubmittedUtc { get; set; } = DateTimeOffset.UtcNow;
        public List<CatalogNoteResponse> Notes { get; set; } = new();
    }
}