using AssignmentProject.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentProject.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {

        protected readonly IConfiguration Configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("APIDB"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Book { get; set; }
    }
}
