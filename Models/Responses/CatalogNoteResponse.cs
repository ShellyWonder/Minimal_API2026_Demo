namespace MinimalAPI2026Demo.Models.Responses
{
    public class CatalogNoteResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public string Author { get; set; } = string.Empty;
    }
}
