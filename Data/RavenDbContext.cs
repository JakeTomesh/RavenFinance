using Microsoft.EntityFrameworkCore;
using RavenFinance.Models;

namespace RavenFinance.Data
{
    public class RavenDbContext : DbContext
    {
      
        public RavenDbContext(DbContextOptions<RavenDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the User entity
            modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                FirstName = "Jake",
                LastName = "Tomesh",
                Email = "jacobtomesh2@gmail.com",
                Password = "sesame"
            }
        );
        }
    } 
}
