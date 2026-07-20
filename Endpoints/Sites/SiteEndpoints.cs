namespace MinimalAPI2026Demo.Endpoints.Sites
{
    public static class SiteEndpoints
    {
        public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder route)
        {
            #region Groups
            var publicGroup = route.MapGroup("/api/public/sites")
               .AllowAnonymous()
                .WithSummary("Public Site Endpoints.")
                .WithDescription("Returns publically available site data")
                .WithTags("Sites - Public")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            var privateGroup = route.MapGroup("/api/private/sites")
                .WithSummary("Private Site Endpoints.")
                .WithDescription("Returns auth-protected site data")
                .WithTags("Sites - Private")
                .AddEndpointFilter<ExceptionHandlingFilter>()
                .RequireAuthorization();

            #endregion

            #region Get Endpoints
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

            //  Private endpoints for authorized users
            privateGroup.MapGet("", GetAllPrivateSites)
                .WithName(nameof(GetAllPrivateSites))
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get all private sites.")
                .WithDescription("Returns a list of all auth-protected site data");

            privateGroup.MapGet("/{id:int}", GetPrivateSiteById)
                .WithName(nameof(GetPrivateSiteById))
                .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get auth-protected site by id")
                .WithDescription("Returns auth-protected site data on a specific private site using the unique site id.");
            #endregion

            #region Posts
            privateGroup.MapPost("", CreateSite)
               .WithName(nameof(CreateSite))
               .Accepts<CreateSiteRequest>("Application/json")
               .Produces<PrivateSiteResponse>(StatusCodes.Status201Created)
               .ProducesValidationProblem()
               .Produces(StatusCodes.Status401Unauthorized)
               .Produces(StatusCodes.Status403Forbidden)
               .Produces(StatusCodes.Status500InternalServerError)
               .WithSummary("Create site by authorized user.")
               .WithDescription("Returns a new site object created by an authorized user.");
            #endregion

            return route;
        }
        #region Handlers
        private static async Task<Ok<List<PublicSiteResponse>>> GetAllPublicSites(ISiteService service, CancellationToken ct)
        {
            return TypedResults.Ok(await service.GetAllSitesPublicAsync(ct));
        }

        private static async Task<Results<Ok<PublicSiteResponse>, NotFound>> GetPublicSiteById(int id, ISiteService service, CancellationToken ct)
        {
            var site = await service.GetPublicSiteByIdAsync(id, ct);
            if (site is null) return TypedResults.NotFound();

            return TypedResults.Ok(site);
        }

        private static async Task<Ok<List<PrivateSiteResponse>>> GetAllPrivateSites(ISiteService service, CancellationToken ct)
        {
            return TypedResults.Ok(await service.GetAllPrivateSitesAsync(ct));
        }

        private static async Task<Results<Ok<PrivateSiteResponse>, NotFound>> GetPrivateSiteById(int id, ISiteService service, CancellationToken ct)
        {
            var site = await service.GetPrivateSiteByIdAsync(id, ct);
            if (site is null) return TypedResults.NotFound();

            return TypedResults.Ok(site);
        }
        private static async Task<Results<Created<PrivateSiteResponse>, ValidationProblem>> CreateSite(CreateSiteRequest request, ISiteService service, CancellationToken ct)
        {
           
            var createdSite = await service.CreateSiteAsync(request, ct);

           
            return TypedResults.Created($"/api/private/sites/{createdSite.Id}",createdSite);
        }


        #endregion
    }
}
