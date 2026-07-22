namespace MinimalAPI2026Demo.Endpoints.Artifacts
{
    public static class ArtifactMediaFileEndpoints
    {
        public static IEndpointRouteBuilder MapArtifactMediaEndpoints(this IEndpointRouteBuilder route)
        {
            #region Groups
            var publicGroup = route.MapGroup("api/public/artifacts/images")
                .AllowAnonymous()
                .WithSummary("Public Artifact Media Files Endpoints.")
                .WithDescription("Returns publically available media file data.")
                .WithTags("Artifact Media Files - Public")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            var privateGroup = route.MapGroup("api/private/artifacts/{artifactId}/images")
                .RequireAuthorization()
                .WithSummary("Private Artifact Media File Endpoints.")
                .WithDescription("Returns private media file data requiring auth user")
                .WithTags("Artifact Media Files - Private")
                .AddEndpointFilter<ExceptionHandlingFilter>();
            #endregion

            #region Get Endpoints
            publicGroup.MapGet("/{id:int}", GetPublicArtifactImage)
                        .WithName(nameof(GetPublicArtifactImage))
                        .Produces<FileContentHttpResult>(StatusCodes.Status200OK)
                        .Produces(StatusCodes.Status404NotFound)
                        .Produces(StatusCodes.Status500InternalServerError)
                        .WithSummary("Get Artifact Image (Public)")
                        .WithDescription("""
                            Retrieves binary image data for a specific artifact media record.
                            This endpoint does not require authentication.
                            All unhandled exceptions are processed by the ExceptionHandlingFilter.
                            """);
            #endregion

            #region Create Endpoints
            privateGroup.MapPost("", CreateArtifactMediaFile)
                        .DisableAntiforgery()
                        .WithName(nameof(CreateArtifactMediaFile))
                        .Accepts<IFormFile>("multipart/form-data")
                        .Produces(StatusCodes.Status201Created)
                        .Produces(StatusCodes.Status400BadRequest)
                        .Produces(StatusCodes.Status404NotFound)
                        .Produces(StatusCodes.Status500InternalServerError)
                         .WithSummary("Upload Artifact Image (Private)")
                        .WithDescription("""
                            Uploads an image file and associates it with an existing artifact.
                            Optional flag 'isPrimary' can be provided in the query or form data.
                            Requires authentication. All unhandled errors are processed by the ExceptionHandlingFilter.
                            """
                        );
            #endregion

            return route;
        }

        #region Handlers
        private static async Task<Results<Created, NotFound, BadRequest>> CreateArtifactMediaFile(int artifactId,
                                                                                                  IFormFile file,
                                                                                                  bool isPrimary,
                                                                                                  IArtifactMediaFileService service,
                                                                                                 CancellationToken ct)
        {
            if (file is null || file.Length == 0) return TypedResults.BadRequest();

            ArtifactMediaFile? media = await service.CreateArtifactMediaFileAsync(artifactId, file, isPrimary, ct);

            if (media is null) return TypedResults.NotFound();

            var location = $"/api/public/artifacts/images/{media.Id}";

            return TypedResults.Created(location);
        }

        private static async Task<Results<FileContentHttpResult, NotFound>> GetPublicArtifactImage(int id,
                                                                                ApplicationDbContext db,
                                                                                HttpResponse response,
                                                                                CancellationToken ct)
        {
            //Method not using a service because no business logic is executed - just a pull from db;
            var image = await db.MediaFiles
                                .AsNoTracking()
                                .FirstOrDefaultAsync(m => m.Id == id, ct);
            if (image == null || image.Data.Length == 0) return TypedResults.NotFound();

            // Add client-side caching for performance
            response.Headers.CacheControl = "public, max-age=86400"; // Cache for 1 day

            return TypedResults.File(image.Data, image.ContentType);
        }

        #endregion
    }
}
