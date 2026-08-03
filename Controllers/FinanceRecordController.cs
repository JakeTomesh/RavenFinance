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
                    PaymentSources = ListPaymentSourcesForViewModel
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
                        PaymentSources = ListPaymentSourcesForViewModel
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
        public IActionResult PaymentSource(int paymentSourceId, int financeRecordId)
        {
            //recieves id for payment source
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
                        return RedirectToAction("PaymentSource", new { item.PaymentSourceId, item.FinanceRecordId});
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
            //save new draft of finance record
            try
            {
                FinanceRecord record = HelperUser.GetFinanceRecordById(dbContext, financeRecordId, user.UserId);
                HelperUser.UpdateFinanceRecord(dbContext, record);

                //draft saved
                //calculate gross balance for finance record
                JsonObject obj = HelperUser.CalculateGrossFinanceRecordValue(dbContext, record);
                //put json into view model for savings?

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
                return View("Index");
            }
            
            
            //gather saving info
            //forward to savings page for user to contribute to savings.


            List<Saving> savings = HelperUser.GetSavingsByUserId(dbContext, user.UserId);

            return View();
        }
    }

    
}
