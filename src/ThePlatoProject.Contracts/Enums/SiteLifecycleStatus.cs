namespace ThePlatoProject.Contracts.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SiteLifecycleStatus
    {
        Active = 1,
        ClosureRequested = 2,
        ClosureAuthorized = 3,
        Closed = 4
    }
}

