using ThePlatoProject.Contracts.Authentication;
using ThePlatoProject.Contracts.Authorization;

namespace ThePlatoProject.Client.Authorization;

public sealed class PlatoUiAuthorizationService
    : IPlatoUiAuthorizationService
{
    public bool CanViewInternalSite(UserInfo user, int siteId) =>
        AppRoles.HasOrganizationWideReadScope(user.Role) ||
        IsAssignedOnSiteEmployee(user, siteId);

    public bool CanUpdateSite(UserInfo user, SitePermissionContext site) =>
        site.LifecycleStatus != SitePermissionState.Closed &&
        (user.Role == AppRoles.Admin ||
         (user.Role == AppRoles.SiteManager &&
          user.AssignedSiteId == site.SiteId));

    public bool CanRequestSitePublicAccess(
        UserInfo user,
        SitePermissionContext site) =>
        user.Role == AppRoles.SiteManager &&
        user.AssignedSiteId == site.SiteId &&
        site.LifecycleStatus == SitePermissionState.Active &&
        !site.IsPublic &&
        site.VerificationStatus != VerificationPermissionState.Pending;

    public bool CanPublishSite(UserInfo user, SitePermissionContext site) =>
        user.Role == AppRoles.Admin && !site.IsPublic;

    public bool CanRequestSiteClosure(
        UserInfo user,
        SitePermissionContext site) =>
        user.Role == AppRoles.SiteManager &&
        user.AssignedSiteId == site.SiteId &&
        site.LifecycleStatus == SitePermissionState.Active;

    public bool CanAuthorizeSiteClosure(
        UserInfo user,
        SitePermissionContext site) =>
        user.Role == AppRoles.Admin &&
        site.LifecycleStatus == SitePermissionState.ClosureRequested;

    public bool CanCompleteSiteClosure(
        UserInfo user,
        SitePermissionContext site) =>
        user.Role is AppRoles.Admin or AppRoles.Archivist &&
        site.LifecycleStatus == SitePermissionState.ClosureAuthorized &&
        site.NonWarehousedArtifactCount == 0;

    public bool CanManageArtifact(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        !IsProcessingLocked(artifact) &&
        (user.Role == AppRoles.Admin ||
         (user.Role == AppRoles.SiteManager &&
          user.AssignedSiteId == artifact.SiteId));

    public bool CanRequestArtifactPublicAccess(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role == AppRoles.SiteManager &&
        user.AssignedSiteId == artifact.SiteId &&
        !IsProcessingLocked(artifact) &&
        artifact.CustodyStatus == ArtifactPermissionState.OnSite &&
        !artifact.IsPublic &&
        artifact.VerificationStatus != VerificationPermissionState.Pending;

    public bool CanPublishArtifact(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role == AppRoles.Admin && !artifact.IsPublic;

    public bool CanRequestArtifactTransfer(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role == AppRoles.SiteManager &&
        user.AssignedSiteId == artifact.SiteId &&
        artifact.CustodyStatus == ArtifactPermissionState.OnSite;

    public bool CanAuthorizeArtifactTransfer(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role == AppRoles.Admin &&
        artifact.CustodyStatus == ArtifactPermissionState.TransferRequested;

    public bool CanMarkShipmentSent(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role == AppRoles.SiteManager &&
        user.AssignedSiteId == artifact.SiteId &&
        artifact.CustodyStatus == ArtifactPermissionState.TransferAuthorized;

    public bool CanRecordWarehouseReceipt(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role is AppRoles.Admin or AppRoles.Archivist &&
        artifact.CustodyStatus == ArtifactPermissionState.InTransit &&
        artifact.ShipmentReceivedAtUtc is null;

    public bool CanRecordWarehouseLocation(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role is AppRoles.Admin or AppRoles.Archivist &&
        artifact.CustodyStatus == ArtifactPermissionState.InTransit &&
        artifact.ShipmentReceivedAtUtc is not null &&
        string.IsNullOrWhiteSpace(artifact.WarehouseLocation);

    public bool CanCompleteArtifactTransfer(
        UserInfo user,
        ArtifactPermissionContext artifact) =>
        user.Role is AppRoles.Admin or AppRoles.Archivist &&
        artifact.CustodyStatus == ArtifactPermissionState.InTransit &&
        artifact.ShipmentReceivedAtUtc is not null &&
        !string.IsNullOrWhiteSpace(artifact.WarehouseLocation);

    private static bool IsAssignedOnSiteEmployee(
        UserInfo user,
        int siteId) =>
        AppRoles.IsOnSiteRole(user.Role) &&
        user.AssignedSiteId == siteId;

    private static bool IsProcessingLocked(
        ArtifactPermissionContext artifact) =>
        artifact.SiteLifecycleStatus == SitePermissionState.Closed ||
        artifact.CustodyStatus is ArtifactPermissionState.InTransit
            or ArtifactPermissionState.Warehoused;
}
