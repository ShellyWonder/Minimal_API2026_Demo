namespace MinimalAPI2026Demo.Endpoints.CatalogRecords
{
    public  static class CatalogRecordsEndpoints
    {
        public static IEndpointRouteBuilder MapCatalogRecordsEndpoints(this IEndpointRouteBuilder route)
        {
            #region Groups
            var publicGroup = route.MapGroup("api/public/catalogrecords")
                .AllowAnonymous()
                .WithTags("Catalog Records - Public")
                .WithSummary("Public Catalog Records Endpoints")
                .WithDescription("Returns publically available catalog records.")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            var privateGroup = route.MapGroup("api/private/catalogrecords")
                .RequireAuthorization()
                .WithTags("Catalog Records - Private")
                .WithSummary("Private Catalog Records Endpoints")
                .WithDescription("Returns private catalog records - authorization required.")
                .AddEndpointFilter<ExceptionHandlingFilter>();
            #endregion

            #region Get Endpoints
            privateGroup.MapGet("/{artifactId:int}", GetCatRecordsByArtifact)
            .WithName(nameof(GetCatRecordsByArtifact))
            .WithSummary("Get all catalog records associated with a specific artifact id  - Authorized Access Only")
            .WithDescription("Returns private data on a specific artifact using the unique site id. Authorization required")
            .Produces<CatalogRecordResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

            #endregion

            #region Create | Update | Delete

            #endregion

            return route;
        }


        #region Handlers


        #region Get Record by Id
        private static async Task<Results<Ok<List<CatalogRecordResponse>>,NotFound>> GetCatRecordsByArtifact(int artifactId,
                                                                                                ICatalogRecordsService service,
                                                                                                CancellationToken ct)
        {
            var records = await service.GetAllPrivateCatRecordsByArtifactIdAsync(artifactId, ct);
            if (records is null ||records.Count == 0) return TypedResults.NotFound();
            return TypedResults.Ok(records);
        }

        #endregion

        #region Get Records <List>

        #endregion

        #region Create | Update | Delete

        #endregion

        #endregion

    }
}
