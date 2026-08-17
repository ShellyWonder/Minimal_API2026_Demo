using MinimalAPI2026Demo.Endpoints.IdentityEndpoints;

namespace MinimalAPI2026Demo.Extensions.ProgramExtensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UsePlatoProjectPipeline(this WebApplication app)
        {
            app.UseBlazorFrameworkFiles();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<BlockIdentityEndpoints>();
            app.UseMiddleware<ActiveEmployeeMiddleware>();
            
            return app;
        }

        public static WebApplication MapPlatoProjectEndpoints(this WebApplication app)
        {
            var authRouteGroup = app.MapGroup("api/auth").WithTags("Authentication");
            authRouteGroup.MapIdentityApi<ApplicationUser>();
            app.MapCustomIdentityEndpoints(); // Map custom identity endpoints
            app.MapCurrentUserEndpoints(); // Map current user endpoints
            app.MapHomeEndpoints(); // Map Home endpoints
            app.MapSiteEndpoints(); // Map Site endpoints
            app.MapArtifactEndpoints(); // Map Artifact endpoints
            app.MapArtifactMediaEndpoints(); //Map Artifact Image endpoints
            app.MapCatalogRecordsEndpoints(); // Map Catalog Record endpoints
            app.MapFallbackToFile("index.html");
            return app;
        }
    }
}
