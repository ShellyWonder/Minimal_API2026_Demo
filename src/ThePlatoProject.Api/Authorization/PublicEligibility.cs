using System.Linq.Expressions;

namespace MinimalAPI2026Demo.Authorization;

public static class PublicEligibility
{
    public static Expression<Func<Site, bool>> Sites =>
        site => site.IsPublic &&
                site.VerificationStatus == VerificationStatus.Verified;

    public static Expression<Func<Artifact, bool>> Artifacts =>
        artifact => artifact.IsPublic &&
                    artifact.VerificationStatus == VerificationStatus.Verified &&
                    artifact.Site != null &&
                    artifact.Site.IsPublic &&
                    artifact.Site.VerificationStatus ==
                        VerificationStatus.Verified;

    public static Expression<Func<ArtifactMediaFile, bool>> MediaFiles =>
        media => media.VerificationStatus == VerificationStatus.Verified &&
                 media.Artifact.IsPublic &&
                 media.Artifact.VerificationStatus ==
                    VerificationStatus.Verified &&
                 media.Artifact.Site != null &&
                 media.Artifact.Site.IsPublic &&
                 media.Artifact.Site.VerificationStatus ==
                    VerificationStatus.Verified;
}
