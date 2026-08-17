namespace ThePlatoProject.Contracts.Authorization;

public static class AppPolicies
{
    public const string ManageSites = nameof(ManageSites);
    public const string ViewPrivateArtifacts = nameof(ViewPrivateArtifacts);
    public const string VerifyArtifactMedia = nameof(VerifyArtifactMedia);
    public const string ManageEmployees = nameof(ManageEmployees);
    public const string ViewAuditLog = nameof(ViewAuditLog);
}
