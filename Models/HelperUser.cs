using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RavenFinance.Data;
using System.Text.RegularExpressions;

namespace RavenFinance.Models
{
    public static class HelperUser
    {
        public static int GetUserFromSession(HttpContext httpContext)
        {
            int? userId = httpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                return userId.Value;
            }
            throw new InvalidOperationException("User ID not found in session.");
        }

        public static User? ValidateCurrentUser(RavenDbContext context, HttpContext session)
        {
            try
            {
                var userId = HelperUser.GetUserFromSession(session);
                if (userId > 0)
                {
                    User? user = HelperUser.GetUserById(context, userId);
                    return user;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating user in session.", ex);
            }
        }

        public static User? IsValidLogin(RavenDbContext context, string Email, string Password)
        {
            if(Email != null && Password != null)
            {
                // Check if the email and password match a user in the database
                var user = context.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
                return user;
            }

            return null;
        }

        public static User ValidateRegister(RavenDbContext context, string Email, string Password, string FirstName, string LastName)
        {
            if (Email == null || Password == null)
            {
                throw new ArgumentNullException("Email and Password cannot be null");
            }

            bool isValidEmail = Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!isValidEmail)
            {
                throw new ArgumentException("Invalid email format. Please provide a valid email address.", nameof(Email));
            }

            var existingUser = context.Users.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("An account with this email address already exists.");
            }
            // If the email is valid and not already in use, create a new user
            //hash password here before saving to database !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            };
            context.Users.Add(newUser);
            context.SaveChanges();
            return newUser;
        }

        public static User? GetUserById(RavenDbContext context, int id)
        {
            try
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == id);
                return user;
            }catch (Exception ex)
            {
                throw new Exception("User not found with the provided ID.", ex);
            }
            
        }
        public static List<RecurringBill> GetRecurringBillsByUserId(RavenDbContext context, int userId)
        {
            try
            {
                var bills = context.RecurringBills
                    .Include(b => b.Category)
                    .Include(b => b.PaymentSource)
                    .Where(b => b.UserId == userId)
                    .ToList();
                
                return bills;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving recurring bills for the user.", ex);
            }
        }

        public static void CreateNewRecurringBill(RavenDbContext context, RecurringBill bill)
        {
            try
            {
                context.RecurringBills.Add(bill);
                context.SaveChanges();
            }catch (Exception ex)
            {
                throw new Exception("Error saving recurring bill to database.", ex);
            }
        }

        public static List<Category> GetCategoriesByUserId(RavenDbContext context, int userId)
        {
            
            try
            {
                List<Category> categories = context.Categories.Where(c => c.UserId == userId).ToList();
                
                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving categories for user.", ex);
            }
            
        }

        public static void CreateNewCategory(RavenDbContext context, Category category)
        {
            try
            {
                context.Categories.Add(category);
                context.SaveChanges();
            }catch (Exception ex)
            {
                throw new Exception("Error creating category.", ex);
            }
        }

        public static void DeleteCategory(RavenDbContext context, Category category)
        {
            try
            {
                context.Categories.Remove(category);
                context.SaveChanges();
            }catch (Exception ex)
            {
                throw new Exception("Error deleting categoy", ex);
            }
        }

        public static Category? GetCategoryByCategoryIdAndUserId(RavenDbContext context, int categoryId, int userId)
        {
            try
            {
                Category? category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId &&
                c.UserId == userId);
                return category;
            }catch (Exception ex)
            {
                throw new Exception("Error retrieving category.", ex);
            }
        }

        public static void UpdateCategory(RavenDbContext context, Category category)
        {
            try
            {
               
                context.Categories.Update(category);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error updating category", ex);
            }
        }

        public static List<PaymentSource> GetPaymentSourcesByUserId(RavenDbContext context, int userId)
        {
            try
            {
                List<PaymentSource> paymentSources = context.PaymentSources.Where(p => p.UserId == userId).ToList();
                return paymentSources;

            }catch(Exception ex)
            {
                throw new Exception("Error retrieving payment source for user.", ex);
            }
        }

        public static void CreateNewPaymentSource(RavenDbContext context, PaymentSource paymentSource)
        {
            try
            {
                context.PaymentSources.Add(paymentSource);
                context.SaveChanges();
            }catch (Exception ex)
            {
                throw new Exception("Error creating new payment source", ex);
            }
        }

        public static PaymentSource GetPaymentSourceById(RavenDbContext context, int paymentSourceId, int userId)
        {
            try
            {
                PaymentSource? paymentSource = context.PaymentSources.FirstOrDefault(p => p.PaymentSourceId == paymentSourceId &&
                    p.UserId == userId);
                if(paymentSource == null)
                {
                    throw new NullReferenceException();
                }
                return paymentSource;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving payment source.", ex);
            }
        }

        public static void UpdatePaymentSource(RavenDbContext context, PaymentSource paymentSource)
        {
            try
            {
                context.PaymentSources.Update(paymentSource);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error updating payment source", ex);
            }
        }

        public static void DeletePaymentSource(RavenDbContext context, PaymentSource paymentSource)
        {
            try
            {
                context.PaymentSources.Remove(paymentSource);
                context.SaveChanges();
            }catch( Exception ex)
            {
                throw new Exception("Error removing payment source.", ex);
            }
        }

    }
}
