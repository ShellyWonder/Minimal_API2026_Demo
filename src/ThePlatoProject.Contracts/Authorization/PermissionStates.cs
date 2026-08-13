namespace ThePlatoProject.Contracts.Authorization;

// UI-facing states are intentionally independent of API entity enums so the
// Contracts project never needs to reference the API project.
public enum SitePermissionState
{
    Active = 1,
    ClosureRequested = 2,
    ClosureAuthorized = 3,
    Closed = 4
}

public enum ArtifactPermissionState
{
    OnSite = 1,
    TransferRequested = 2,
    TransferAuthorized = 3,
    InTransit = 4,
    Warehoused = 5
}

public enum VerificationPermissionState
{
    Unverified = 1,
    Pending = 2,
    Verified = 3,
    Rejected = 4
}
