namespace ThePlatoProject.Client.Authorization.Records;

public sealed record AccountResult(
    bool Succeeded,
    string? ErrorMessage = null);
