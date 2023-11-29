using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AssignmentProject.Core.Entities;

namespace AssignmentProject.Helpers
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("APIDB"));
        }

        public DbSet<User> Users { get; set; }
    }
}