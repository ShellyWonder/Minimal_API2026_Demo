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
            .WithSummary("Retrieve a single catalog record by its database ID - Authorized Access Only")
            .WithDescription("Returns private data on a specific artifact using the unique site id. Authorization required")
            .Produces<CatalogRecordResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
            #endregion

            #region Create | Update | Delete
            privateGroup.MapPost("", CreateCatalogRecord)
            .WithName(nameof(CreateCatalogRecord))
            .WithDescription("Creates a single catalog record associated with an artifact ID - Authorized Access Only")
            .WithSummary("Create a single catalog record - Authorization required")
            .Produces<CatalogRecordResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapPut("/{id:int}", UpdateCatalogRecord)
            .WithName(nameof(UpdateCatalogRecord))
            .WithDescription("Updates a single catalog record associated with an artifact ID - Authorized Access Only")
            .WithSummary("Update a single catalog record - Authorization required")
            .Accepts<UpdateCatalogRecordRequest>("Application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

            
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
        private static async Task<Results<Created<CatalogRecordResponse>, BadRequest>> CreateCatalogRecord(ClaimsPrincipal user,
                                                                                                    CreateCatalogRecordRequest request,
                                                                                                    ICatalogRecordsService service,
                                                                                                    CancellationToken ct)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return TypedResults.BadRequest();

            var created = await service.CreateCatalogRecordAsync(userId, request, ct);
            if (created is null) return TypedResults.BadRequest();

            return TypedResults.Created($"/api/private/catalogrecord/{created.Id}", created);

        }

        private static async Task<Results<NoContent, NotFound>> UpdateCatalogRecord(int id,
                                                                                   UpdateCatalogRecordRequest request,
                                                                                   ICatalogRecordsService service,
                                                                                   CancellationToken ct)
        {
            var success = await service.UpdateCatalogRecordAsync(id, request, ct);

            if (!success) return TypedResults.NotFound();
                    return TypedResults.NoContent();
        }


        #endregion

        #endregion

    }
}
