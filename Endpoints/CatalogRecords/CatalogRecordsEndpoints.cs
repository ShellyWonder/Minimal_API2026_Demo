namespace MinimalAPI2026Demo.Endpoints.CatalogRecords
{
    public static class CatalogRecordsEndpoints
    {
        public static IEndpointRouteBuilder MapCatalogRecordsEndpoints(this IEndpointRouteBuilder route)
        {
            #region Groups
            
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
            .WithDescription("Returns private data on a specific artifact collection(List) using the unique site id. Authorization required")
            .Produces<CatalogRecordResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapGet("/cr{id:int}", GetCatRecordById)
            .WithName(nameof(GetCatRecordById))
            .WithSummary("Retrieves a single catalog record by its database ID - Authorized Access Only")
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

        #region Get Record by Record Id
        private static async Task<Results<Ok<CatalogRecordResponse>, NotFound>> GetCatRecordById(int id,
                                                                                            ICatalogRecordsService service,
                                                                                            CancellationToken ct)
        {
            var record = await service.GetCatalogRecordByIdAsync(id, ct);
            if (record is null) return TypedResults.NotFound();

            return TypedResults.Ok(record);
        }
        #endregion

        #region Get Records by Artifact Id <List>
        private static async Task<Results<Ok<List<CatalogRecordResponse>>, NotFound>> GetCatRecordsByArtifact(int artifactId,
                                                                                        ICatalogRecordsService service,
                                                                                        CancellationToken ct)
        {
            var records = await service.GetAllPrivateCatRecordsByArtifactIdAsync(artifactId, ct);
            if (records is null || records.Count == 0) return TypedResults.NotFound();
            return TypedResults.Ok(records);
        }

        #endregion

        #region Create | Update | Delete

        #endregion

        #endregion

    }
}
