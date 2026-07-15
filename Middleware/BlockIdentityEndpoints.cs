namespace MinimalAPI2026Demo.Middleware
{


    public class BlockIdentityEndpoints
    {
        private readonly RequestDelegate _next;

        // Define which routes to block
        private static readonly string[] _blockedPaths =
        [
            "/api/auth/register",
            "/api/auth/resetpassword",
            "/api/auth/forgotpassword",
            "/api/auth/manage/info",
            "/api/auth/manage/profile"
        ];

        public BlockIdentityEndpoints(RequestDelegate next) => _next = next;

        //fires on every request to check if the path is blocked
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            if (path is not null && _blockedPaths.Contains(path))
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
