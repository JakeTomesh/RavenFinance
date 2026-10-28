using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RavenFinance.Data;
using System.Text.Json.Nodes;
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

        public static List<PaymentSourceSummaryViewModel> GetPaymentSourcesForViewModel(RavenDbContext context, int userId, int financeRecordId)
        {
            try
            {
                List<PaymentSourceSummaryViewModel> paymentSources = context.PaymentSources
                    .Where(ps => ps.UserId == userId)
                    .Select(ps => new PaymentSourceSummaryViewModel
                    {
                        PaymentSourceId = ps.PaymentSourceId,
                        Name = ps.Name,

                        StatementItemCount = ps.StatementItems.Count(si =>
                            si.FinanceRecordId == financeRecordId &&
                            si.UserId == userId)
                    })
                    .ToList();
                return paymentSources;

            }catch(Exception ex)
            {
                throw new Exception("Error retrieving payment sources for view model.", ex);
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

        public static RecurringBill GetRecurringBillByBillId(RavenDbContext context, int recurringBillId, int userId)
        {
            try
            {
               RecurringBill? bill = context.RecurringBills
                    .Include(b => b.Category)
                    .Include(b => b.PaymentSource)
                    .FirstOrDefault(b => b.RecurringBillId == recurringBillId && b.UserId == userId);
                if (bill == null)
                    return null;
                else
                {
                    return bill;
                }
            }catch(Exception ex)
            {
                throw new Exception("Error loading recurring bill.", ex);
            }
        }

        public static void UpdateRecurringBill(RavenDbContext context,  RecurringBill recurringBill)
        {
            try
            {
                context.RecurringBills.Update(recurringBill);
                context.SaveChanges();

            }catch(Exception ex)
            {
                throw new Exception("Error updating recurring bill.", ex);
            }
        }

        public static void DeleteRecurringBill(RavenDbContext context, int recurringBillId, int userId)
        {
            try
            {
                var bill = GetRecurringBillByBillId(context, recurringBillId, userId);

                context.RecurringBills.Remove(bill);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error deleting recurring bill.", ex);
            }
        }

        public static List<Saving> GetSavingsByUserId(RavenDbContext context, int userId)
        {
            try
            {
                List<Saving> savings = context.Savings.Where(s => s.UserId == userId).ToList();

                // Load contributions and savings-paid expenses separately, skipping soft-deleted records,
                // so Saving.Balance only reflects live records. Tracked entities fix up the navigations.
                context.SavingContributions
                    .Where(c => c.UserId == userId && !c.FinanceRecord!.IsDeleted)
                    .Load();
                context.StatementItems
                    .Where(si => si.UserId == userId && si.SavingId != null && !si.FinanceRecord!.IsDeleted)
                    .Load();
                return savings;
            }catch(Exception ex)
            {
                throw new Exception("Error retrieving user's savings.", ex);
            }
        }

        public static void CreateNewSaving(RavenDbContext context, Saving saving)
        {
            try
            {
                context.Savings.Add(saving);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error saving new savings.", ex);
            }
        }

        public static Saving GetSavingById(RavenDbContext context, int savingId, int userId)
        {
            try
            {
                Saving? saving = context.Savings.FirstOrDefault(s => s.SavingId == savingId && s.UserId == userId);
                return saving;
            }catch(Exception ex)
            {
                throw new Exception("Error retrieving savings.", ex);
            }
        }

        public static void DeleteSaving(RavenDbContext context, Saving saving)
        {
            try
            {
                context.Savings.Remove(saving);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error deleting savings.", ex);
            }
        }

        public static void UpdateSaving(RavenDbContext context, Saving saving)
        {
            try
            {
                context.Savings.Update(saving);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error updating savings.", ex);
            }
        }

        public static void CreateFinanceRecord(RavenDbContext context, FinanceRecord record)
        {
            try
            {
                context.FinanceRecords.Add(record);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error updating Finance Record.", ex);
            }
        }

        public static void UpdateFinanceRecord(RavenDbContext context, FinanceRecord record)
        {
            try
            {
                context.FinanceRecords.Update(record);
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Error updating finance record.", ex);
            }
        }
        
        public static FinanceRecord GetFinanceRecordById(RavenDbContext context, int financeRecordId, int userId)
        {
            try
            {
                FinanceRecord? record = context.FinanceRecords.FirstOrDefault(f => f.FinanceRecordId == financeRecordId && f.UserId == userId && !f.IsDeleted);
                
                if(record == null)
                    throw new NullReferenceException(nameof(record));
                return record;
            }catch(Exception ex)
            {
                throw new Exception("Error retrieving finance record.", ex);
            }
        }

        public static FinanceRecord GetFinanceRecordWithAllRelations(RavenDbContext context, int financeRecordId, int userId)
        {
            try
            {
                var financeRecord = context.FinanceRecords
                    .Where(fr =>
                        fr.FinanceRecordId == financeRecordId &&
                        fr.UserId == userId)
                    .Include(fr => fr.StatementItems)
                        .ThenInclude(si => si.PaymentSource)
                    .Include(fr => fr.StatementItems)
                        .ThenInclude(si => si.Category)
                    .FirstOrDefault();

                if (financeRecord == null)
                    throw new NullReferenceException(nameof(financeRecord));
                return financeRecord;
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving finance record.", ex);
            }
        }

        // Current balance of a savings account: live contributions minus live savings-paid expenses.
        // excludeStatementItemId lets an edit ignore the item's own previous amount.
        public static decimal GetSavingBalance(RavenDbContext context, int savingId, int userId, int excludeStatementItemId = 0)
        {
            decimal initial = context.Savings
                .Where(s => s.SavingId == savingId && s.UserId == userId)
                .Select(s => (decimal?)s.Amount).FirstOrDefault() ?? 0m;
            decimal contributed = context.SavingContributions
                .Where(c => c.SavingId == savingId && c.UserId == userId && !c.FinanceRecord!.IsDeleted)
                .Sum(c => (decimal?)c.Amount) ?? 0m;
            decimal spent = context.StatementItems
                .Where(s => s.SavingId == savingId && s.UserId == userId
                         && s.ItemType == StatementItem.StatementItemType.Expense
                         && s.StatementItemId != excludeStatementItemId
                         && !s.FinanceRecord!.IsDeleted)
                .Sum(s => (decimal?)s.Amount) ?? 0m;
            return initial + contributed - spent;
        }

        public static void CreateNewStatementItem(RavenDbContext context, StatementItemCreateViewModel model, int userId)
        {
            //one way to throw null exceptions without if statement
            //pretty cool

            ArgumentNullException.ThrowIfNull(model);
            try
            {
                StatementItem item = new StatementItem
                {
                    //id genereated on save
                    Description = model.Description,
                    Amount = model.Amount,
                    Merchant = model.Merchant,
                    DateOfTransaction = model.DateOfTransaction,
                    ItemType = (StatementItem.StatementItemType)model.ItemType,
                    IsMonthlyRecurringBill = model.IsMonthlyRecurringBill,
                    CategoryId = model.CategoryId,
                    UserId = userId,
                    PaymentSourceId = model.SavingId.HasValue ? null : model.PaymentSourceId,
                    SavingId = model.SavingId,
                    FinanceRecordId = model.FinanceRecordId,
                };

                context.StatementItems.Add(item);
                context.SaveChanges();


            }catch(Exception ex)
            {
                throw new Exception("Error saving statement item.", ex);
            }
        }

        public static void CleanUpUserDrafts(RavenDbContext context, int userId)
        {
            try
            {
                // Only abandoned drafts with no statement items are removed; drafts the user has
                // worked on are kept so they can be resumed from the dashboard.
                var records = context.FinanceRecords
                    .Where(f => f.UserId == userId && f.IsComplete == false && !f.IsDeleted
                             && !context.StatementItems.Any(s => s.FinanceRecordId == f.FinanceRecordId));

                if(records != null)
                {
                    foreach (var item in records)
                    {
                        context.Remove(item);
                    }
                    context.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing user's drafts.", ex);
            }
        }

        // Adds the user's recurring bills to a newly created finance record as expense statement items.
        // Each item goes on the bill's own PaymentSource, so the normal totals (net before savings),
        // payment source counts, review page and edit/remove flow all include them automatically.
        // One item is created for every month in the record's date range where the bill's due day falls inside the range.
        public static void AddRecurringBillsToFinanceRecord(RavenDbContext context, FinanceRecord record, int userId)
        {
            try
            {
                var bills = context.RecurringBills.Where(b => b.UserId == userId).ToList();
                var newItems = new List<StatementItem>();

                foreach (var bill in bills)
                {
                    var month = new DateOnly(record.DateStart.Year, record.DateStart.Month, 1);
                    while (month <= record.DateEnd)
                    {
                        // Clamp the due day so e.g. the 31st becomes the 28th/30th in shorter months
                        int day = Math.Clamp(bill.DueDate, 1, DateTime.DaysInMonth(month.Year, month.Month));
                        var dueDate = new DateOnly(month.Year, month.Month, day);

                        if (dueDate >= record.DateStart && dueDate <= record.DateEnd)
                        {
                            newItems.Add(new StatementItem
                            {
                                Description = bill.Notes,
                                Amount = bill.Amount,
                                Merchant = bill.Name,
                                DateOfTransaction = dueDate,
                                ItemType = bill.ItemType,
                                IsMonthlyRecurringBill = true,
                                CategoryId = bill.CategoryId,
                                UserId = userId,
                                PaymentSourceId = bill.PaymentSourceId,
                                FinanceRecordId = record.FinanceRecordId
                            });
                        }
                        month = month.AddMonths(1);
                    }
                }

                if (newItems.Count > 0)
                {
                    context.StatementItems.AddRange(newItems);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding recurring bills to finance record.", ex);
            }
        }

        // Keeps the user's RecurringBill list in sync with the statement items they flagged as recurring.
        // Called when a finance record is confirmed, so future records pick the bills up automatically.
        //  - A flagged expense matching an existing bill (same name + payment source) updates that bill.
        //  - Otherwise a new RecurringBill is created.
        // Only expenses are synced, since recurring bills are expenses. Does not save; the caller saves
        // so the sync and completing the record happen in one transaction.
        public static void SyncRecurringBillsFromFinanceRecord(RavenDbContext context, FinanceRecord record, int userId)
        {
            try
            {
                var flagged = context.StatementItems
                    .Where(s => s.FinanceRecordId == record.FinanceRecordId
                             && s.UserId == userId
                             && s.IsMonthlyRecurringBill
                             && s.PaymentSourceId != null && s.SavingId == null
                             && s.Merchant != null && s.Merchant != "")
                    .ToList();

                var existingBills = context.RecurringBills.Where(b => b.UserId == userId).ToList();

                // One bill per name + payment source; the latest item wins if the range spans several months
                var groups = flagged.GroupBy(s => new { Name = s.Merchant!.Trim().ToLower(), s.PaymentSourceId, s.ItemType });

                foreach (var g in groups)
                {
                    StatementItem latest = g.OrderByDescending(s => s.DateOfTransaction).First();
                    string name = latest.Merchant!.Trim();

                    RecurringBill? bill = existingBills.FirstOrDefault(b =>
                        b.PaymentSourceId == latest.PaymentSourceId &&
                        b.ItemType == latest.ItemType &&
                        string.Equals(b.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));

                    if (bill != null)
                    {
                        bill.Amount = latest.Amount;
                        bill.ItemType = latest.ItemType;
                        bill.CategoryId = latest.CategoryId;                        bill.DueDate = latest.DateOfTransaction.Day;
                        bill.Notes = latest.Description;
                    }
                    else
                    {
                        context.RecurringBills.Add(new RecurringBill
                        {
                            UserId = userId,
                            Name = name,
                            Amount = latest.Amount,
                            ItemType = latest.ItemType,
                            DueDate = latest.DateOfTransaction.Day,
                            CategoryId = latest.CategoryId,
                            PaymentSourceId = latest.PaymentSourceId!.Value,
                            Notes = latest.Description
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error syncing recurring bills.", ex);
            }
        }

        public static void CreateSavingContribution(RavenDbContext context, int savingId, int financeRecordId, int userId, decimal amount)
        {
            try
            {
                context.SavingContributions.Add(new SavingContribution
                {
                    SavingId = savingId,
                    FinanceRecordId = financeRecordId,
                    UserId = userId,
                    Amount = amount,
                    DateCommitted = DateTime.UtcNow
                });
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving contribution to savings.", ex);
            }
        }

        public static StatementItem? GetStatementItemById(RavenDbContext context, int statementItemId, int userId)
        {
            try
            {
                return context.StatementItems.FirstOrDefault(s => s.StatementItemId == statementItemId && s.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving statement item.", ex);
            }
        }

        // Applies edits from the view model to an existing statement item owned by the user.
        // PaymentSourceId and FinanceRecordId are intentionally not changed.
        public static void UpdateStatementItem(RavenDbContext context, StatementItemCreateViewModel model, int userId)
        {
            ArgumentNullException.ThrowIfNull(model);
            try
            {
                StatementItem item = GetStatementItemById(context, model.StatementItemId, userId)
                    ?? throw new NullReferenceException("Statement item not found.");

                item.Description = model.Description;
                item.Amount = model.Amount;
                item.Merchant = model.Merchant;
                item.DateOfTransaction = model.DateOfTransaction;
                item.ItemType = (StatementItem.StatementItemType)model.ItemType;
                item.IsMonthlyRecurringBill = model.IsMonthlyRecurringBill;
                item.CategoryId = model.CategoryId;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating statement item.", ex);
            }
        }

        public static void DeleteStatementItem(RavenDbContext context, int statementItemId, int userId)
        {
            try
            {
                StatementItem item = GetStatementItemById(context, statementItemId, userId)
                    ?? throw new NullReferenceException("Statement item not found.");
                context.StatementItems.Remove(item);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting statement item.", ex);
            }
        }

        public static SavingContribution? GetSavingContributionById(RavenDbContext context, int contributionId, int userId)
        {
            try
            {
                return context.SavingContributions.FirstOrDefault(c => c.SavingContributionId == contributionId && c.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving savings contribution.", ex);
            }
        }

        // Builds the full review model: items grouped by payment source, contributions, and totals.
        public static ReviewFinanceRecordViewModel BuildReviewViewModel(RavenDbContext context, FinanceRecord record)
        {
            try
            {
                var items = context.StatementItems
                    .Include(s => s.PaymentSource)
                    .Include(s => s.Saving)
                    .Include(s => s.Category)
                    .Where(s => s.FinanceRecordId == record.FinanceRecordId)
                    .OrderBy(s => s.DateOfTransaction)
                    .ToList();

                var contributions = context.SavingContributions
                    .Include(c => c.Saving)
                    .Where(c => c.FinanceRecordId == record.FinanceRecordId)
                    .OrderBy(c => c.DateCommitted)
                    .ToList();

                JsonObject totals = CalculateGrossFinanceRecordValue(context, record);

                return new ReviewFinanceRecordViewModel
                {
                    FinanceRecordId = record.FinanceRecordId,
                    DateStart = record.DateStart,
                    DateEnd = record.DateEnd,
                    IsComplete = record.IsComplete,
                    // Savings-paid items form their own group per savings account (PaymentSourceId 0)
                    PaymentSourceGroups = items
                        .GroupBy(s => new
                        {
                            s.SavingId,
                            PaymentSourceId = s.PaymentSourceId ?? 0,
                            Name = s.Saving != null ? s.Saving.Name + " (Savings)" : (s.PaymentSource?.Name ?? "Unknown")
                        })
                        .Select(g => new ReviewPaymentSourceGroup
                        {
                            PaymentSourceId = g.Key.PaymentSourceId,
                            PaymentSourceName = g.Key.Name,
                            Items = g.ToList()
                        }).ToList(),
                    Contributions = contributions,
                    DepositTotal = totals["depositTotal"]?.GetValue<decimal>() ?? 0m,
                    ExpenseTotal = totals["expenseTotal"]?.GetValue<decimal>() ?? 0m,
                    NetTotal = totals["netTotal"]?.GetValue<decimal>() ?? 0m,
                    SavingsTotal = totals["savingsTotal"]?.GetValue<decimal>() ?? 0m,
                    SavingsSpentTotal = totals["savingsSpentTotal"]?.GetValue<decimal>() ?? 0m,
                    RemainingTotal = totals["remainingTotal"]?.GetValue<decimal>() ?? 0m
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error building finance record review.", ex);
            }
        }

        public static JsonObject CalculateGrossFinanceRecordValue(RavenDbContext context, FinanceRecord record)
        {
            try
            {
                List<StatementItem> statementItems = context.StatementItems.Where(s => s.FinanceRecordId == record.FinanceRecordId).ToList();

                JsonObject json = new JsonObject();
                decimal depositTotal = 0;
                decimal expenseTotal = 0;
                decimal netTotal = 0;

                var depositItems = statementItems.Where(s => s.ItemType == StatementItem.StatementItemType.Deposit).ToList();
                // Expenses paid from a savings account are funded by that account, not by this record's
                // income, so they are excluded from the expense total and net balance (tracked separately).
                var expenseItems = statementItems.Where(r => r.ItemType == StatementItem.StatementItemType.Expense && r.SavingId == null).ToList();
                decimal savingsSpentTotal = statementItems
                    .Where(r => r.ItemType == StatementItem.StatementItemType.Expense && r.SavingId != null)
                    .Sum(r => r.Amount);

                foreach (var item in depositItems)
                {
                    depositTotal += item.Amount;
                }
                foreach (var item in expenseItems)
                {
                    expenseTotal += item.Amount;
                }

                netTotal = depositTotal - expenseTotal;

                // Savings already committed for this finance record reduce the balance still available
                decimal savingsTotal = context.SavingContributions
                    .Where(c => c.FinanceRecordId == record.FinanceRecordId)
                    .Sum(c => c.Amount);

                json["depositTotal"] = depositTotal;
                json["expenseTotal"] = expenseTotal;
                json["netTotal"] = netTotal;
                json["savingsTotal"] = savingsTotal;
                json["savingsSpentTotal"] = savingsSpentTotal;
                // Balance still available to commit to savings
                json["remainingTotal"] = netTotal - savingsTotal;


                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving statement items.", ex);
            }
        }
    }
}
