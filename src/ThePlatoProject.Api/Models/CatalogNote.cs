namespace MinimalAPI2026Demo.Models
{
    public class CatalogNote
    {
        public int Id { get; set; }
        [Required]
        public int CatalogRecordId { get; set; } //foreign key
        public CatalogRecord CatalogRecord { get; set; } = null!; //nav property
        public string AuthorId { get; set; } = string.Empty; //foreign key
        public ApplicationUser Author { get; set; } = null!; //Nav property

        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; } = DateTime.UtcNow;
    }
}