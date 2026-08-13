namespace ThePlatoProject.Contracts.Authorization;

// These are UI authorization inputs, not new persisted entities.
public sealed record SitePermissionContext(
    int SiteId,
    bool IsPublic,
    VerificationPermissionState VerificationStatus,
    SitePermissionState LifecycleStatus,
    int NonWarehousedArtifactCount);

public sealed record ArtifactPermissionContext(
    int ArtifactId,
    int SiteId,
    SitePermissionState SiteLifecycleStatus,
    bool IsPublic,
    VerificationPermissionState VerificationStatus,
    ArtifactPermissionState CustodyStatus,
    DateTimeOffset? ShipmentReceivedAtUtc,
    string? WarehouseLocation);
