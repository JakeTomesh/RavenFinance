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
        public DbSet<PaymentSource> PaymentSources { get; set; }
        public DbSet<Saving> Savings { get; set; }
        public DbSet<FinanceRecord> FinanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatementItem>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RecurringBill>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            //modelBuilder.Entity<User>()
            //     .HasData(
            //     new User
            //     {
            //         UserId = 2,
            //         FirstName = "Jake",
            //         LastName = "Tomesh",
            //         Email = "jacobtomesh2@gmail.com",
            //         Password = "sesame"
            //     });
            
            
        }


    } 
}
