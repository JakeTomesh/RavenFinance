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
        public DbSet<StatementItem> StatementItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FinanceRecord -> StatementItems: Cascade delete
            modelBuilder.Entity<FinanceRecord>()
                .HasMany(fr => fr.StatementItems)
                .WithOne(si => si.FinanceRecord)
                .HasForeignKey(si => si.FinanceRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> StatementItems: Restrict to avoid multiple cascade paths
            modelBuilder.Entity<User>()
                .HasMany<StatementItem>()
                .WithOne(si => si.User)
                .HasForeignKey(si => si.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // PaymentSource -> StatementItems: Restrict
            modelBuilder.Entity<PaymentSource>()
                .HasMany(ps => ps.StatementItems)
                .WithOne(si => si.PaymentSource)
                .HasForeignKey(si => si.PaymentSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> FinanceRecords: Restrict to avoid cascade conflicts
            modelBuilder.Entity<User>()
                .HasMany<FinanceRecord>()
                .WithOne(fr => fr.User)
                .HasForeignKey(fr => fr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> PaymentSources: Restrict to avoid cascade conflicts
            modelBuilder.Entity<User>()
                .HasMany<PaymentSource>()
                .WithOne(ps => ps.User)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category -> StatementItems: Restrict to avoid cascade conflicts
            modelBuilder.Entity<Category>()
                .HasMany<StatementItem>()
                .WithOne(si => si.Category)
                .HasForeignKey(si => si.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StatementItem>()
                .Property(si => si.Amount)
                .HasPrecision(18, 2);

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
