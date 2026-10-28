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
        public DbSet<SavingContribution> SavingContributions { get; set; }

        /// <summary>Dashboard to-do notes, stored in the "Task" table.</summary>
        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Task list: table is named "Task"; a user's tasks are removed with the user
            modelBuilder.Entity<TaskItem>().ToTable("Task");
            modelBuilder.Entity<User>()
                .HasMany<TaskItem>()
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Saving -> Contributions: Restrict (avoid multiple cascade paths)
            modelBuilder.Entity<Saving>()
                .HasMany(s => s.Contributions)
                .WithOne(c => c.Saving)
                .HasForeignKey(c => c.SavingId)
                .OnDelete(DeleteBehavior.Restrict);

            // FinanceRecord -> Contributions: Cascade delete with the record
            modelBuilder.Entity<FinanceRecord>()
                .HasMany<SavingContribution>()
                .WithOne(c => c.FinanceRecord)
                .HasForeignKey(c => c.FinanceRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavingContribution>()
                .Property(c => c.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Saving>()
                .Property(s => s.Amount)
                .HasPrecision(18, 2);

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
