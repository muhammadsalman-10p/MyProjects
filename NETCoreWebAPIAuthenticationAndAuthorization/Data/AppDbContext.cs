using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NETCoreWebAPI.Data.Models;

namespace NETCoreWebAPI.Data
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options ) : base(options)
        {
                
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; } //manually added to create refresh token table in db
    }
}
