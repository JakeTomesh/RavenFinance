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
        public DbSet<RecurringBill> RecurringBills { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatementItem>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RecurringBill>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);
            
        }


    } 
}
