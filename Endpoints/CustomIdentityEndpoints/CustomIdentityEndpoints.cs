using Microsoft.AspNetCore.Identity.Data;
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
            group.MapPost("/register-admin", RegisterUser)  // ("/route", handler method)
                 .WithName("RegisterUser")
                 .WithSummary("Register a new user")
                 .WithDescription("This endpoint registers a user in an admin role.")
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest);
            //.RequireAuthorization("AdminPolicy"); 

            group.MapPost("/reset-password", ResetPassword)  // ("/route", handler method)
                 .WithName("ResetPassword")
                 .WithDescription("This endpoint allows users to reset their password.")
                 .WithSummary("Reset user password")
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapPost("/forgot-password", ForgotPassword)  // ("/route", handler method)
                 .WithName("ForgotPassword")
                 .WithDescription("This endpoint orchestrates forgot password workflow.")
                 .WithSummary("Forgot user password")
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest);


            return route;
        }
        #region Handler Methods
        private static async Task<IResult> ForgotPassword(ForgotPasswordRequest request,
                                                    UserManager<ApplicationUser> userManager,
                                                    IEmailSender emailSender,
                                                    IConfiguration config)
        {
            if (string.IsNullOrEmpty(request.Email)) 
                return Results.BadRequest((new { Message = "Email is required." }));

            //Find the user by email
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Results.Ok(new { Message = "If user exists a reset password link will be sent." });

            //Generate a resetToken
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            //Encode the token to make it URL-safe
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

            var baseURL = config["BaseUrl"] ?? "https://localhost:7166"; //Route to the frontend page for email confirmation
            var resetLink = $"{baseURL}/reset-password?email={request.Email}&resetCode={encodedToken}";

            //TODO: Implement a proper email sending service and template for production use
            await emailSender.SendEmailAsync(request.Email,
                 "Reset your password.",
                $"""
               
                Please reset your password by clicking this link: <a href='{resetLink}'>link</a>
                """
                );

            // Return the route result with the custom identity information
            return Results.Ok(new { Message = "If user exists a reset password link will be sent" });

        }
      
        // Handler methods for the custom identity info endpoint
        private static async Task<IResult> ResetPassword(ResetPasswordRequest request,
                                                        UserManager<ApplicationUser> userManager)
        {
            //Check user inputs are valid
            if (string.IsNullOrEmpty(request.Email)
                || string.IsNullOrEmpty(request.ResetCode)
                || string.IsNullOrEmpty(request.NewPassword))
                return Results.BadRequest(new { Message = "All fields are required." });
            //Find the user by email
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Results.BadRequest(new { Message = "User not found." });
            try
            {
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
                var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
               
                if(!result.Succeeded) 
                    return Results.BadRequest(new { Message = $"Password reset failed: {string.Join(", ", result.Errors.Select(e => e.Description))}" });
                else               
                    return Results.Ok(new { Message = "Password reset successfully." });
                
            }
            catch (FormatException)
            {
                return Results.BadRequest(new { Message = $"Invalid Token Format" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Message = $"Error resetting password: {ex.Message}" });
            }

        }
        private static async Task<IResult> RegisterUser(RegisterUserRequest request,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IConfiguration config
            )
        {
            //Check if the user already exists
            if (await userManager.FindByEmailAsync(request.Email) is not null)
                return Results.BadRequest($"User with email {request.Email} already exists.");

            //Create a new ApplicationUser instance with the provided information
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            var TempPassword = config["DefaultUserPassword"] ?? "TempP@ssw0rd123$";
            IdentityResult created = await userManager.CreateAsync(user, TempPassword);

            if (!created.Succeeded)
                return Results.BadRequest(new { Message = "Failed to create user." });

            if (await roleManager.RoleExistsAsync("Researcher"))
                await userManager.AddToRoleAsync(user, "Researcher");
            //Generate password reset token for the user
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            //Encode the token to make it URL-safe
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

            //Send a confirmation email to the user 
            var baseUrl = config["BaseUrl"] ?? "https://localhost:7166"; //Route to the frontend page for email confirmation
            //TODO: Implement a proper email sending service and template for production use
            await emailSender.SendEmailAsync(request.Email,
                "Confirm your email",
                $"""
                Your account has been created.
                Please confirm your account by clicking this link: <a href='{baseUrl}/confirm-email?email={request.Email}'>link</a>");
                {baseUrl}/SetPassword.html?email={request.Email}&resetCode={encodedToken}
                """
                );

            // Return the route result with the custom identity information
            return Results.Ok(new { Message = $"User {user.Email} registered successfully. Password reset link sent to email." });
        }

       
        #endregion
    }
}
