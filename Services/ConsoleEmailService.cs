using Microsoft.AspNetCore.Identity.UI.Services;

namespace MinimalAPI2026Demo.Services
{
    public class ConsoleEmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Sending email to: {email}\nSubject: {subject}\nMessage: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
