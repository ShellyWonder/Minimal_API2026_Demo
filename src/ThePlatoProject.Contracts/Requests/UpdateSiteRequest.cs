namespace ThePlatoProject.Contracts.Requests
{
    
    public class UpdateSiteRequest 
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Coordinates =>
            FormattableString.Invariant($"{Latitude:F6}, {Longitude:F6}");

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? PublicNarrative { get; set; }

        [MaxLength(2000)]
        public string? ALRECNarrative { get; set; }
    }
}
    

