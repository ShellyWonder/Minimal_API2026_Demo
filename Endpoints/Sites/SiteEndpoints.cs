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
                .WithTags("Sites - Public")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            publicGroup.MapGet("", GetAllPublicSites)
                .WithName(nameof(GetAllPublicSites))
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get all public sites.")
                .WithDescription("Returns a list of all publically available site data");

            publicGroup.MapGet("/{id:int}", GetPublicSiteById)
                .WithName(nameof(GetPublicSiteById))
                .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get public site by id")
                .WithDescription("Returns publically available site data on a specific site using the unique site id.");

            return route;
        }
        #region Handlers
        private static async Task<Ok<List<PublicSiteResponse>>> GetAllPublicSites(ISiteService service, CancellationToken ct)
        {
            return TypedResults.Ok(await service.GetAllSitesPublicAsync(ct));
        }

        private static async Task<Results<Ok<PublicSiteResponse>, NotFound>>GetPublicSiteById(int id, ISiteService service, CancellationToken ct)
        {
            var site = await service.GetPublicSiteByIdAsync(id, ct);
            if (site is null) return TypedResults.NotFound();

            return TypedResults.Ok(site);
        }
        #endregion
    }
}
