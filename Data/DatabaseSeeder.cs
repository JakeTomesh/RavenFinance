using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(RavenDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Users only if none exist
            if (!context.Users.Any())
            {
                var testUser = new User
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@test.com",
                    Password = "password123" // TODO: Hash this password!
                };
                context.Users.Add(testUser);
                context.SaveChanges();
            }

            // Seed Categories only if none exist
            if (!context.Categories.Any())
            {
                var userId = context.Users.First().UserId;
                var categories = new List<Category>
                {
                    new Category { Name = "Utilities", UserId = userId },
                    new Category { Name = "Subscriptions", UserId = userId },
                    new Category { Name = "Insurance", UserId = userId },
                    new Category { Name = "Housing", UserId = userId }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed Payment Sources only if none exist
            if (!context.PaymentSources.Any())
            {
                var userId = context.Users.First().UserId;
                var paymentSources = new List<PaymentSource>
                {
                    new PaymentSource { Name = "Checking Account", UserId = userId },
                    new PaymentSource { Name = "Credit Card", UserId = userId },
                    new PaymentSource { Name = "Savings Account", UserId = userId }
                };
                context.PaymentSources.AddRange(paymentSources);
                context.SaveChanges();
            }

            // You can add more seed data here as needed
        }
    }
}
