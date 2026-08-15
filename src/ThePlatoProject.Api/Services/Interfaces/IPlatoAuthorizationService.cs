using System.Security.Claims;

namespace MinimalAPI2026Demo.Authorization;

public interface IPlatoAuthorizationService
{
    Task<bool> CanViewSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct);

    Task<bool> CanManageSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct);

    Task<bool> CanViewArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct);

    Task<bool> CanManageArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct);

    Task<bool> CanVerifyArtifactMediaAsync(
        ClaimsPrincipal principal,
        int mediaId,
        CancellationToken ct);

    Task<bool> CanRequestSitePublicAccessAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct);

    Task<bool> CanRequestArtifactPublicAccessAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct);

    Task<bool> CanPublishSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct);

    Task<bool> CanPublishArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct);

    Task<bool> CanUnpublishSiteAsync(
        ClaimsPrincipal principal,
        int siteId,
        CancellationToken ct);

    Task<bool> CanUnpublishArtifactAsync(
        ClaimsPrincipal principal,
        int artifactId,
        CancellationToken ct);
}
