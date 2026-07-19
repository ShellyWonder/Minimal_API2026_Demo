namespace MinimalAPI2026Demo.Models
{
    public class ArtifactMediaFile
    {
        public int Id { get; set; }
        public int ArtifactId { get; set; }//Foreign key
        public Artifact Artifact { get; set; } = null!; //navigation Property
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "image/jpeg";
        public byte[] Data { get; set; } = []; //image stored as a byte array in db
        public bool IsPrimary { get; set; } = false; //mark image as primary (or "main") if more than one

    }
}