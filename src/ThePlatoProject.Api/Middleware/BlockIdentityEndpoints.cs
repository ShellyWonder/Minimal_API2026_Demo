namespace MinimalAPI2026Demo.Middleware
{


    public class BlockIdentityEndpoints(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        // Define which routes to block
        private static readonly string[] _blockedPaths =
        [
            "/api/auth/register",
            "/api/auth/reinitpassword",
            "/api/auth/forgotpassword",
            "/api/auth/resetPassword",
            "/api/auth/manage/info",
            "/api/auth/manage/profile"
        ];

        public static string[] BlockedPaths => _blockedPaths;

        //fires on every request to check if the path is blocked
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            if (path is not null && BlockedPaths.Contains(path))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = $"Endpoint '{path}' is disabled for extended Identity."
                });
                return; // stop the pipeline here
            }

            await _next(context); // move to the next middleware
        }
    }

}
