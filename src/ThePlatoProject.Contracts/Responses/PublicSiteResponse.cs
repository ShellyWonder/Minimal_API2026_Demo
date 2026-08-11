namespace ThePlatoProject.Contracts.Responses
{
   
    public class PublicSiteResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Coordinates =>
            FormattableString.Invariant($"{Latitude:F6}, {Longitude:F6}");
        public string?  Description { get; set; }
        public string?  PublicNarrative { get; set; }

    }
}
