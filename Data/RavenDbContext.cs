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
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });
        }
    } 
}
