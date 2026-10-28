using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RavenFinance.Data;
using RavenFinance.Models;
using System.Text.Json.Nodes;

namespace RavenFinance.Controllers
{
    public class FinanceRecordController : Controller
    {
        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public FinanceRecordController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database
        public IActionResult Index()
        {
            return RedirectToAction("DateSelection");
        }

        //Index redirects to DateSelection as the fisrt step in the construction of FinanceRecord obj
        [HttpGet]
        public IActionResult DateSelection()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPaymentSourceSelectionView(int financeRecordId)
        {
            if(financeRecordId < 0)
            {
                TempData["ErrorMessage"] = "Invalide finance record.";
                return RedirectToAction("Index", "FinanceRecord");
            }
            try
            {
                var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                if(user == null)
                {
                    HttpContext.Session.Clear();
                    TempData["ErrorMessage"] = "Invalid user, please login to continue";
                    return RedirectToAction("Index", "Login");
                }

                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                if(record == null)
                {
                    
                    TempData["ErrorMessage"] = "Error retrieving finance record.";
                    return RedirectToAction("Index");
                }

                //get all payment sources for a user
                List<PaymentSource> userPaymentSources = HelperUser.GetPaymentSourcesByUserId(dbContext, user.UserId);

                //init list to populate view model
                List<PaymentSourceSummaryViewModel> ListPaymentSourcesForViewModel = new List<PaymentSourceSummaryViewModel>();

                //create a temp view model for each payment source for a user
                //append to list for view model
                foreach(var item in userPaymentSources)
                {
                    PaymentSourceSummaryViewModel paymentSourceViewModel = new PaymentSourceSummaryViewModel
                    {
                        PaymentSourceId = item.PaymentSourceId,
                        Name = item.Name,
                        IsActive = item.IsActive,
                        StatementItemCount = dbContext.StatementItems
                            .Count(si => si.PaymentSourceId == item.PaymentSourceId 
                                      && si.FinanceRecordId == financeRecordId)
                    };
                    ListPaymentSourcesForViewModel.Add(paymentSourceViewModel);
                }

                FinanceRecordPaymentSourceViewModel recordViewModel = new FinanceRecordPaymentSourceViewModel
                {
                    FinanceRecordId = record.FinanceRecordId,
                    PaymentSources = ListPaymentSourcesForViewModel,
                    SavingSources = HelperUser.GetSavingsByUserId(dbContext, user.UserId)
                        .Select(s => new SavingSourceSummaryViewModel
                        {
                            SavingId = s.SavingId,
                            Name = s.Name,
                            Balance = s.Balance,
                            StatementItemCount = dbContext.StatementItems
                                .Count(si => si.SavingId == s.SavingId && si.FinanceRecordId == financeRecordId)
                        }).ToList()
                };

                return View("PaymentSource", recordViewModel);
            }catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult DateSelection(FinanceRecord record)
        {
            if (ModelState.IsValid)
            {
                if(record.DateStart > record.DateEnd)
                {
                    ViewBag.ErrorMessage = "Date start cannot be after date end.";
                    return View("DateSelection");
                }

                var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                if(user != null){
                    record.UserId = user.UserId;
                    //build user PaymentSource and Saving into obj

                    List<PaymentSource> userPaymentSource = HelperUser.GetPaymentSourcesByUserId(dbContext, user.UserId);

                    List<Saving> userSavings = HelperUser.GetSavingsByUserId(dbContext, user.UserId);


                    foreach(Saving item in userSavings)
                    {
                        if(item != null)
                        {
                            record.Saving.Add(item);
                        }
                    }
                    //save FinanceRecord obj to db as draft for cancel navigation/state holding
                    try
                    {
                        HelperUser.CreateFinanceRecord(dbContext, record);
                        //append the user's recurring bills (on their own payment sources) before any totals are calculated
                        HelperUser.AddRecurringBillsToFinanceRecord(dbContext, record, user.UserId);
                    }catch(Exception ex)
                    {
                        TempData["ErrorMessage"] = ex;
                        return View("Index");
                    }


                    //successful draft save
                    if (record == null)
                    {
                        TempData["ErrorMessage"] = "Error retrieving finance record.";
                        return RedirectToAction("Index");
                    }

                    //get all payment sources for a user
                    List<PaymentSource> userPaymentSources = HelperUser.GetPaymentSourcesByUserId(dbContext, user.UserId);

                    //init list to populate view model
                    List<PaymentSourceSummaryViewModel> ListPaymentSourcesForViewModel = new List<PaymentSourceSummaryViewModel>();

                    //create a temp view model for each payment source for a user
                    //append to list for view model
                    foreach (var item in userPaymentSources)
                    {
                        PaymentSourceSummaryViewModel paymentSourceViewModel = new PaymentSourceSummaryViewModel
                        {
                            PaymentSourceId = item.PaymentSourceId,
                            Name = item.Name,
                            IsActive = item.IsActive,
                            StatementItemCount = dbContext.StatementItems
                                .Count(si => si.PaymentSourceId == item.PaymentSourceId 
                                          && si.FinanceRecordId == record.FinanceRecordId)
                                //returns count of statement items related to the finance record id
                        };
                        ListPaymentSourcesForViewModel.Add(paymentSourceViewModel);
                    }

                    FinanceRecordPaymentSourceViewModel recordViewModel = new FinanceRecordPaymentSourceViewModel
                    {
                        FinanceRecordId = record.FinanceRecordId,
                        PaymentSources = ListPaymentSourcesForViewModel,
                        SavingSources = HelperUser.GetSavingsByUserId(dbContext, user.UserId)
                            .Select(s => new SavingSourceSummaryViewModel
                            {
                                SavingId = s.SavingId,
                                Name = s.Name,
                                Balance = s.Balance,
                                StatementItemCount = dbContext.StatementItems
                                    .Count(si => si.SavingId == s.SavingId && si.FinanceRecordId == record.FinanceRecordId)
                            }).ToList()
                    };

                    return View("PaymentSource", recordViewModel);

                }
                else
                {
                    HttpContext.Session.Clear();
                    TempData["ErrorMessage"] = "Invalid user, please login to continue";
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid financial record";
                return RedirectToAction("DateSelection");
            }
        }

        [HttpGet]
        public IActionResult PaymentSource(int paymentSourceId, int financeRecordId, int? savingId)
        {
            //recieves id for payment source, or a savings account id acting as an expense-only source
            if(paymentSourceId < 0)
            {
                ViewBag.ErrorMessage = "Invalid payment source.";
                return View("PaymentSource");
            }

            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if(user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            if (savingId.HasValue)
            {
                Saving? saving = dbContext.Savings.FirstOrDefault(s => s.SavingId == savingId.Value && s.UserId == user.UserId);
                if (saving == null)
                {
                    TempData["ErrorMessage"] = "Invalid savings account.";
                    return RedirectToAction("GetPaymentSourceSelectionView", new { financeRecordId });
                }

                var savingViewModel = new StatementItemCreateViewModel
                {
                    FinanceRecordId = financeRecordId,
                    SavingId = saving.SavingId,
                    ItemType = StatementItemCreateViewModel.StatementItemType.Expense,
                    CategoryOptions = HelperUser.GetCategoriesByUserId(dbContext, user.UserId).Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    }).ToList(),
                    Amount = 00.00M,
                    DateOfTransaction = DateOnly.FromDateTime(DateTime.Today)
                };

                ViewData["PaymentSourceName"] = saving.Name + " (Savings)";
                return View("StatementItem", savingViewModel);
            }
           
            try
            {
                PaymentSource paymentSource = HelperUser.GetPaymentSourceById(dbContext, paymentSourceId, user.UserId);
                if (paymentSource == null)
                {
                    TempData["ErrorMessage"] = "Invalid payment source.";
                    return RedirectToAction("Index", "PaymentSource");
                }

                List<Category> categories = HelperUser.GetCategoriesByUserId(dbContext, user.UserId);

                var viewModel = new StatementItemCreateViewModel
                {
                    //default values for view model
                    FinanceRecordId = financeRecordId,
                    PaymentSourceId = paymentSource.PaymentSourceId,
                    CategoryOptions = categories.Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    }).ToList(),
                    Amount = 00.00M,
                    DateOfTransaction = DateOnly.FromDateTime(DateTime.Today),
                    IsMonthlyRecurringBill = false

                };


                ViewData["PaymentSourceName"] = paymentSource.Name;
                return View("StatementItem", viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("PaymentSource");
            }
        

        }


        [HttpPost]
        public IActionResult CreateStatementItem(StatementItemCreateViewModel item, string submitAction)
        
        {

            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            if (item.DateOfTransaction > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(item.DateOfTransaction), "Date must not be in the future.");

            }

            if(item.Merchant == null)
            {
                ModelState.AddModelError(nameof(item.Merchant), "Merchant name is required.");
            }

            if(item.Amount <= 0)
            {
                ModelState.AddModelError(nameof(item.Amount), "Amount must be a positive decimal.");
            }

            if(item.CategoryId <= 0)
            {
                ModelState.AddModelError(nameof(item.CategoryId), "Category selection is required.");
            }

            if (item.SavingId.HasValue)
            {
                Saving? saving = dbContext.Savings.FirstOrDefault(s => s.SavingId == item.SavingId.Value && s.UserId == user.UserId);
                if (saving == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid savings account.");
                }
                else
                {
                    ViewData["PaymentSourceName"] = saving.Name + " (Savings)";
                    decimal balance = HelperUser.GetSavingBalance(dbContext, saving.SavingId, user.UserId);
                    if (item.Amount > balance)
                    {
                        ModelState.AddModelError(nameof(item.Amount), $"Amount cannot exceed the savings balance ({balance:C}).");
                    }
                }

                // Savings sources
                if (item.ItemType != StatementItemCreateViewModel.StatementItemType.Expense)
                {
                    ModelState.AddModelError(nameof(item.ItemType), "Savings accounts can only be used for expenses.");
                }
                item.IsMonthlyRecurringBill = false;
                ModelState.Remove(nameof(item.PaymentSourceId));
            }
            else if (item.PaymentSourceId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid payment source.");
            }


            //final validation of model
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid form submission. Please try again.";
                //re-populate category options
                List<Category> categories = HelperUser.GetCategoriesByUserId(dbContext, user.UserId);
                item.CategoryOptions = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList();

                return View("StatementItem", item);
            }

            var itemHere = item;
            switch (submitAction)
            {
                case "addAnother":
                    //save and repeat form, redirect to PaymentSource GET
                    try
                    {
                        HelperUser.CreateNewStatementItem(dbContext, item, user.UserId);
                        TempData["SuccessMessage"] = "Statement item saved successfully! Continue to add another.";
                        return RedirectToAction("PaymentSource", new { item.PaymentSourceId, item.FinanceRecordId, item.SavingId });
                    }catch(Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.Message;
                        return View("StatementItem", item);
                    }
                    
                case "finish":
                    //save and forward to next view GET
                    try
                    {
                        HelperUser.CreateNewStatementItem(dbContext, item, user.UserId);
                        TempData["SuccessMessage"] = "Statement item saved successfully!";
                        return RedirectToAction("GetPaymentSourceSelectionView", new { item.FinanceRecordId });
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.Message;
                        return View("StatementItem", item);
                    }
                default:
                    {
                        TempData["ErrorMessage"] = "An error occurred while processing the statement item. Please try again.";
                        return RedirectToAction("PaymentSource", new { item.PaymentSourceId, item.FinanceRecordId });
                    }
            }
        }


        [HttpGet]
        public IActionResult Saving(int financeRecordId)
        {
            if(financeRecordId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid finace record.";
                return RedirectToAction("GetPaymentSourceSelectionView", financeRecordId);
            }
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }
            SavingsViewModel viewModel;
            //save new draft of finance record
            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                HelperUser.UpdateFinanceRecord(dbContext, record);

                //draft saved

                //calculate gross balance for finance record
                JsonObject obj = HelperUser.CalculateGrossFinanceRecordValue(dbContext, record);
                //gather all of the user's savings accounts for the select element
                List<Saving> savings = HelperUser.GetSavingsByUserId(dbContext, user.UserId);

                viewModel = new SavingsViewModel
                {
                    FinanceRecordId = financeRecordId,
                    UserId = user.UserId,
                    SavingsOptions = savings,
                    GrossFinanceRecordValue = obj
                };
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
                return View("Index");
            }
            
            
            //gather saving info
            //forward to savings page for user to contribute to savings.


            return View(viewModel);
        }

        // Handles a savings commit. The page cycles through this action (Post-Redirect-Get)
        // until the user chooses to continue.
        // submitAction values:
        //   "commit"         - save the contribution and stay on the savings page
        //   "commitContinue" - save the contribution (if an amount was entered) and move on
        //   "continue"       - move on without contributing
        [HttpPost]
        public IActionResult Saving(SavingsViewModel model, string submitAction)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord? record = model.FinanceRecordId > 0
                    ? HelperUser.GetFinanceRecordById(dbContext, model.FinanceRecordId, user.UserId)
                    : null;
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Invalid finance record.";
                    return RedirectToAction("Index");
                }

                bool wantsToContribute = submitAction != "continue"
                    && !(submitAction == "commitContinue" && model.ContributionAmount == 0);

                if (wantsToContribute)
                {
                    decimal amount = Math.Round(model.ContributionAmount, 2);

                    // The savings account must belong to the current user
                    Saving? saving = HelperUser.GetSavingById(dbContext, model.SelectedSavingId, user.UserId);
                    if (saving == null)
                    {
                        TempData["ErrorMessage"] = "Please select a valid savings account.";
                        return RedirectToAction("Saving", new { financeRecordId = record.FinanceRecordId });
                    }

                    // Remaining balance is always recalculated server side, never trusted from the client
                    JsonObject totals = HelperUser.CalculateGrossFinanceRecordValue(dbContext, record);
                    decimal remaining = totals["remainingTotal"]?.GetValue<decimal>() ?? 0m;

                    if (amount <= 0 || amount > remaining)
                    {
                        TempData["ErrorMessage"] = $"Enter an amount greater than 0 and no more than {remaining:C}.";
                        return RedirectToAction("Saving", new { financeRecordId = record.FinanceRecordId });
                    }

                    HelperUser.CreateSavingContribution(dbContext, saving.SavingId, record.FinanceRecordId, user.UserId, amount);
                    TempData["SuccessMessage"] = $"{amount:C} committed to {saving.Name}.";
                }

                if (submitAction == "commit")
                {
                    // Stay on the savings page; GET recalculates the remaining balance
                    return RedirectToAction("Saving", new { financeRecordId = record.FinanceRecordId });
                }

                return RedirectToAction("ReviewFinanceRecord", new { financeRecordId = record.FinanceRecordId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ---------------------------------------------------------------------
        // Review step: shows the whole finance record and lets the user edit it
        // before confirming. Every action below re-validates the user and that the
        // record/item belongs to them, and refuses changes to completed records.
        // ---------------------------------------------------------------------

        [HttpGet]
        public IActionResult ReviewFinanceRecord(int financeRecordId)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                return View(HelperUser.BuildReviewViewModel(dbContext, record));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult EditRecordDates(int financeRecordId, DateOnly dateStart, DateOnly dateEnd)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                if (dateStart > dateEnd)
                {
                    TempData["ErrorMessage"] = "Date start cannot be after date end.";
                }
                else
                {
                    record.DateStart = dateStart;
                    record.DateEnd = dateEnd;
                    dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Dates updated.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("ReviewFinanceRecord", new { financeRecordId });
        }

        [HttpGet]
        public IActionResult EditStatementItem(int statementItemId)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                StatementItem? item = HelperUser.GetStatementItemById(dbContext, statementItemId, user.UserId);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Statement item not found.";
                    return RedirectToAction("Index");
                }

                var model = new StatementItemCreateViewModel
                {
                    StatementItemId = item.StatementItemId,
                    FinanceRecordId = item.FinanceRecordId,
                    PaymentSourceId = item.PaymentSourceId ?? 0,
                    SavingId = item.SavingId,
                    DateOfTransaction = item.DateOfTransaction,
                    Merchant = item.Merchant,
                    Description = item.Description,
                    Amount = item.Amount,
                    ItemType = (StatementItemCreateViewModel.StatementItemType)item.ItemType,
                    IsMonthlyRecurringBill = item.IsMonthlyRecurringBill,
                    CategoryId = item.CategoryId,
                    CategoryOptions = GetCategoryOptions(user.UserId)
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult EditStatementItem(StatementItemCreateViewModel model)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            if (model.DateOfTransaction > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(model.DateOfTransaction), "Date must not be in the future.");
            }
            if (model.CategoryId <= 0)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category selection is required.");
            }

            try
            {
                // Ownership and completion checks use the stored item, not posted values
                StatementItem? existing = HelperUser.GetStatementItemById(dbContext, model.StatementItemId, user.UserId);
                if (existing == null)
                {
                    TempData["ErrorMessage"] = "Statement item not found.";
                    return RedirectToAction("Index");
                }

                model.FinanceRecordId = existing.FinanceRecordId;
                model.PaymentSourceId = existing.PaymentSourceId ?? 0;
                model.SavingId = existing.SavingId;
                if (existing.SavingId.HasValue && model.ItemType != StatementItemCreateViewModel.StatementItemType.Expense)
                {
                    ModelState.AddModelError(nameof(model.ItemType), "Savings accounts can only be used for expenses.");
                }
                if (existing.SavingId.HasValue)
                {
                    decimal balance = HelperUser.GetSavingBalance(dbContext, existing.SavingId.Value, user.UserId, existing.StatementItemId);
                    if (model.Amount > balance)
                    {
                        ModelState.AddModelError(nameof(model.Amount), $"Amount cannot exceed the savings balance ({balance:C}).");
                    }
                }

                if (!ModelState.IsValid)
                {
                    model.CategoryOptions = GetCategoryOptions(user.UserId);
                    return View(model);
                }

                HelperUser.UpdateStatementItem(dbContext, model, user.UserId);
                TempData["SuccessMessage"] = "Statement item updated.";
                return RedirectToAction("ReviewFinanceRecord", new { financeRecordId = existing.FinanceRecordId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                model.CategoryOptions = GetCategoryOptions(user.UserId);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult DeleteStatementItem(int statementItemId, int financeRecordId)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Ownership of the record is verified (and deleted records rejected) by GetFinanceRecordById
                HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                HelperUser.DeleteStatementItem(dbContext, statementItemId, user.UserId);
                TempData["SuccessMessage"] = "Statement item removed.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("ReviewFinanceRecord", new { financeRecordId });
        }

        // Edits the amount of a savings contribution. The new amount must be > 0 and fit
        // within the balance available once the old amount is released.
        [HttpPost]
        public IActionResult EditSavingContribution(int contributionId, int financeRecordId, decimal amount)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                SavingContribution? contribution = HelperUser.GetSavingContributionById(dbContext, contributionId, user.UserId);
                if (contribution == null || contribution.FinanceRecordId != record.FinanceRecordId)
                {
                    TempData["ErrorMessage"] = "Unable to edit this contribution.";
                }
                else
                {
                    amount = Math.Round(amount, 2);
                    decimal remaining = HelperUser.CalculateGrossFinanceRecordValue(dbContext, record)["remainingTotal"]?.GetValue<decimal>() ?? 0m;
                    decimal maxAllowed = remaining + contribution.Amount;
                    if (amount <= 0 || amount > maxAllowed)
                    {
                        TempData["ErrorMessage"] = $"Enter an amount greater than 0 and no more than {maxAllowed:C}.";
                    }
                    else
                    {
                        contribution.Amount = amount;
                        dbContext.SaveChanges();
                        TempData["SuccessMessage"] = "Contribution updated.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("ReviewFinanceRecord", new { financeRecordId });
        }

        [HttpPost]
        public IActionResult DeleteSavingContribution(int contributionId, int financeRecordId)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                SavingContribution? contribution = HelperUser.GetSavingContributionById(dbContext, contributionId, user.UserId);
                if (contribution == null || contribution.FinanceRecordId != record.FinanceRecordId)
                {
                    TempData["ErrorMessage"] = "Unable to remove this contribution.";
                }
                else
                {
                    dbContext.SavingContributions.Remove(contribution);
                    dbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Contribution removed.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("ReviewFinanceRecord", new { financeRecordId });
        }

        // Final step: marks the draft as complete and persists it.
        [HttpPost]
        public IActionResult ConfirmFinanceRecord(int financeRecordId)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                bool wasComplete = record.IsComplete;

                if (!dbContext.StatementItems.Any(s => s.FinanceRecordId == financeRecordId))
                {
                    TempData["ErrorMessage"] = "Add at least one statement item before confirming.";
                    return RedirectToAction("ReviewFinanceRecord", new { financeRecordId });
                }

                // Never let savings exceed what the record can afford
                // A negative record (expenses > deposits) may be saved as long as nothing was contributed to savings.
                // Contributions are only invalid when they push the remaining balance below zero.
                JsonObject confirmTotals = HelperUser.CalculateGrossFinanceRecordValue(dbContext, record);
                decimal remaining = confirmTotals["remainingTotal"]?.GetValue<decimal>() ?? 0m;
                decimal savingsCommitted = confirmTotals["savingsTotal"]?.GetValue<decimal>() ?? 0m;
                if (savingsCommitted > 0 && remaining < 0)
                {
                    TempData["ErrorMessage"] = "Savings contributions exceed the net balance. Please adjust them before confirming.";
                    return RedirectToAction("ReviewFinanceRecord", new { financeRecordId });
                }

                // Flagged recurring expenses become (or update) the user's RecurringBills,
                // saved together with completing the record
                HelperUser.SyncRecurringBillsFromFinanceRecord(dbContext, record, user.UserId);
                record.IsComplete = true;
                dbContext.SaveChanges();

                TempData["SuccessMessage"] = wasComplete ? "Finance record changes saved." : "Finance record submitted.";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ReviewFinanceRecord", new { financeRecordId });
            }
        }

        // Leaves the record incomplete (IsComplete = false) so it shows on the dashboard as a draft.
        // Everything entered so far is already stored; this just exits the process.
        [HttpPost]
        public IActionResult SaveDraft(int financeRecordId)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                // A completed record stays complete; only incomplete records are "drafts"
                TempData["SuccessMessage"] = record.IsComplete ? "Changes saved." : "Finance record saved as a draft.";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Dashboard");
            }
        }

        // Soft delete: the row is kept but flagged IsDeleted, so it is hidden from the
        // draft and completed lists and shown only as "deleted".
        [HttpPost]
        public IActionResult DeleteFinanceRecord(int financeRecordId)
        {
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                record.IsDeleted = true;
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Finance record deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", "Dashboard");
        }

        private List<SelectListItem> GetCategoryOptions(int userId)
        {
            return HelperUser.GetCategoriesByUserId(dbContext, userId)
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToList();
        }
    }

    
}
