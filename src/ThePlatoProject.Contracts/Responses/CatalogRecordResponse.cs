namespace ThePlatoProject.Contracts.Responses
{
    public class CatalogRecordResponse
    {
        public int Id { get; set; }
        [Required]
        public int ArtifactId { get; set; }
        [Required]
        
        
        public string SubmittedBy { get; set; } = string.Empty;
        public string? VerifiedBy { get; set; } 

        [Required]
        public string Status { get; set; } = "Draft";
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
        public List<CatalogNoteResponse> Notes { get; set; } = new();
    }
}