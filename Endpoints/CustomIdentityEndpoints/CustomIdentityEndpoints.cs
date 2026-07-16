using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace MinimalAPI2026Demo.Endpoints.CustomIdentityEndpoints
{
    public static class CustomIdentityEndpoints
    {
        public static IEndpointRouteBuilder MapCustomIdentityEndpoints(this IEndpointRouteBuilder route)
        {
            //Step 1: Create a new route group for custom identity endpoints
            var group = route.MapGroup("/api/auth").WithTags("Admin");
            //Step 2: Add a custom identity endpoint to the group
            group.MapPost("/register-admin", RegisterUser)
                 .WithName("RegisterUserAsync")
                 .WithSummary("Register a new admin user")
                 .WithDescription("This endpoint registers a user in an admin role.");
            //.RequireAuthorization("AdminPolicy"); 
            return route;
        }
        // Handler method for the custom identity info endpoint
        private static async Task<IResult> RegisterUser(RegisterUserRequest dto,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IConfiguration config
            )
        {
            //Check if the user already exists
            if(await userManager.FindByEmailAsync(dto.Email) is not null)
                            return Results.BadRequest($"User with email {dto.Email} already exists.");
            
            //Create a new ApplicationUser instance with the provided information
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            var TempPassword = config["DefaultUserPassword"] ?? "TempP@ssw0rd123$";
            IdentityResult created = await userManager.CreateAsync(user, TempPassword);

            if(!created.Succeeded)
                       return Results.BadRequest(new { Message = "Failed to create user." });

            if(await roleManager.RoleExistsAsync("Researcher"))
                await userManager.AddToRoleAsync(user, "Researcher");
            //Generate password reset token for the user
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            //Encode the token to make it URL-safe
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

            //Send a confirmation email to the user 
            var baseUrl = config["BaseUrl"] ?? "https://localhost:7166"; //Route to the frontend page for email confirmation
            //TODO: Implement a proper email sending service and template for production use
            await emailSender.SendEmailAsync(dto.Email, 
                "Confirm your email", 
                $"""
                Your account has been created.
                Please confirm your account by clicking this link: <a href='{baseUrl}/confirm-email?email={dto.Email}'>link</a>");
                {baseUrl}/SetPassword.html?email={dto.Email}&resetCode={encodedToken}
                """
                );

            // Return the route result with the custom identity information
            return Results.Ok(new {Message = $"User {user.Email} registered successfully. Password reset link sent to email."});
        }
    }
}
