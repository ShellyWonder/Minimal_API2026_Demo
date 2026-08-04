namespace ThePlatoProject.Contracts.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ArchiveWorkflowState
    {
        None = 0,
        Requested = 1,
        Authorized = 2,
        Archived = 3,
        Rejected = 4

    }
}
