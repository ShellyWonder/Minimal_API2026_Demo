using Microsoft.AspNetCore.Http.HttpResults;

namespace MinimalAPI2026Demo.Endpoints.Home
{
    public static class HomeEndpoints
    {
        public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder route)
        {
            var homeGroup = route.MapGroup("/api/Home").WithTags("Home");
            ///api/Home/welcome
            homeGroup.MapGet("/welcome", GetWelcomeMessage)
                // Add metadata for Swagger documentation
                .WithName("GetWelcomeMessage")
                .WithSummary("Returns a welcome message for the Minimal API demonstration.")
                .WithDescription("Displays a welcome message, version information, and the current server time in a structured JSON format.");
                
                
            return route;
        }

        // Handler method for the welcome endpoint
        private static async Task<Ok<WelcomeResponse>> GetWelcomeMessage(CancellationToken ct)
        {
            var WelcomeMessage = new WelcomeResponse
            {
                Message = "Welcome to the Minimal API demonstration!",
                Version = "1.0.0",
                TimeOnly = DateTime.Now.ToString("T")
            };
            return TypedResults.Ok<WelcomeResponse>(WelcomeMessage);//returns a 200 OK response with the WelcomeResponse object
        }
    }
}
