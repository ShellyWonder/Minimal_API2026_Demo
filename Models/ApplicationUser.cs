
namespace MinimalAPI2026Demo.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }

        public string FullName => $"({FirstName} {LastName})";
    }

}
