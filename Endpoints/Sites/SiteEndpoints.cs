
namespace MinimalAPI2026Demo.Endpoints.Sites
{
    public static class SiteEndpoints
    {
        public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder route)
        {
            var publicGroup = route.MapGroup("/api/public/sites")
               .AllowAnonymous()
                .WithSummary("Public Site Endpoints.")
                .WithDescription("Displays a list of all publically available site data")
                .WithTags("Sites - Public");

            publicGroup.MapGet("", GetAllPublicSites)
                .WithName(nameof(GetAllPublicSites))
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
                .WithSummary("Public Site Endpoints.")
                .WithDescription("Returns a list of all publically available site data");

            return route;
        }
        #region Handlers
        private static async Task<Ok<List<PublicSiteResponse>>> GetAllPublicSites(ISiteService service, CancellationToken ct)
        {
            return TypedResults.Ok(await service.GetAllSitesPublicAsync(ct));
        }
        #endregion
    }
}
