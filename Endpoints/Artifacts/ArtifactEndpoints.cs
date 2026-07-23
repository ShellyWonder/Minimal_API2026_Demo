namespace MinimalAPI2026Demo.Endpoints.Artifacts
{
    public static class ArtifactEndpoints
    {
        public static IEndpointRouteBuilder MapArtifactEndpoints(this IEndpointRouteBuilder route)
        {
            #region Groups
            var publicGroup = route.MapGroup("api/public/artifacts")
                .AllowAnonymous()
                .WithTags("Artifacts - Public")
                .WithSummary("Public Artifact Endpoints.")
                .WithDescription("Returns publically available artifact data.")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            var privateGroup = route.MapGroup("api/private/artifacts")
                .RequireAuthorization()
                .WithTags("Artifacts - Private")
                .WithSummary("Private Artifact Endpoints.")
                .WithDescription("Returns private artifact data requiring auth user.")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            #endregion

            #region Get Endpoints
            publicGroup.MapGet("", GetPublicArtifacts)
           .WithName(nameof(GetPublicArtifacts))
           .WithSummary("Get all public artifacts")
           .WithDescription("Returns all artifacts with public fields and image")
           .Produces<List<PublicArtifactResponse>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound)
           .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapGet("", GetAllPrivateArtifacts)
           .WithName(nameof(GetAllPrivateArtifacts))
           .WithSummary("Get all private artifacts")
           .WithDescription("returns all artifact records with private data")
           .Produces<List<PrivateArtifactResponse>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status404NotFound)
           .Produces(StatusCodes.Status500InternalServerError);
            #endregion

            #region Create Endpoints
            privateGroup.MapPost("", CreateArtifact)
                .WithName(nameof(CreateArtifact))
                .WithSummary("Create new artifact record - authorization required")
                .WithDescription("Creates a new artifact with private data.")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);
            #endregion

            return route;

        }

        #region Handlers
        private static async Task<Results<Created<PrivateArtifactResponse>,NotFound>> CreateArtifact(IArtifactService service,
                                                                                            CreateArtifactRequest request,
                                                                                            CancellationToken ct)
        {
            var created = await service.CreateArtifactAsync(request, ct);
            if (created is null) return TypedResults.NotFound();

            return TypedResults.Created($"/api/private/artifacts/{created.Id}", created);
        }
        private static async Task<Results<Ok<List<PublicArtifactResponse>>,NotFound>> GetPublicArtifacts(IArtifactService service,
                                                                                       CancellationToken ct)
        {
            var artifacts = await service.GetAllPublicArtifactsAsync(ct);

            if (artifacts is null || artifacts.Count == 0) return TypedResults.NotFound();

            return TypedResults.Ok(artifacts);
        }
        private static async Task<Results<Ok<List<PrivateArtifactResponse>>,NotFound>> GetAllPrivateArtifacts(IArtifactService service,
                                                                                       CancellationToken ct)
        {
            var artifacts = await service.GetAllPrivateArtifactsAsync(ct);

            if (artifacts is null || artifacts.Count == 0) return TypedResults.NotFound();

            return TypedResults.Ok(artifacts);
        }
        #endregion

    }
}
