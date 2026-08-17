using ThePlatoProject.Contracts.Authentication;
using ThePlatoProject.Contracts.Authorization;

namespace ThePlatoProject.Client.Authorization;

public interface IPlatoUiAuthorizationService
{
    bool CanViewInternalSite(UserInfo user, int siteId);
    bool CanUpdateSite(UserInfo user, SitePermissionContext site);
    bool CanRequestSitePublicAccess(UserInfo user, SitePermissionContext site);
    bool CanPublishSite(UserInfo user, SitePermissionContext site);
    bool CanRequestSiteClosure(UserInfo user, SitePermissionContext site);
    bool CanAuthorizeSiteClosure(UserInfo user, SitePermissionContext site);
    bool CanCompleteSiteClosure(UserInfo user, SitePermissionContext site);

    bool CanManageArtifact(UserInfo user, ArtifactPermissionContext artifact);
    bool CanRequestArtifactPublicAccess(UserInfo user, ArtifactPermissionContext artifact);
    bool CanPublishArtifact(UserInfo user, ArtifactPermissionContext artifact);
    bool CanRequestArtifactTransfer(UserInfo user, ArtifactPermissionContext artifact);
    bool CanAuthorizeArtifactTransfer(UserInfo user, ArtifactPermissionContext artifact);
    bool CanMarkShipmentSent(UserInfo user, ArtifactPermissionContext artifact);
    bool CanRecordWarehouseReceipt(UserInfo user, ArtifactPermissionContext artifact);
    bool CanRecordWarehouseLocation(UserInfo user, ArtifactPermissionContext artifact);
    bool CanCompleteArtifactTransfer(UserInfo user, ArtifactPermissionContext artifact);
}
