using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace MinimalAPI2026Demo.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

    }
}
