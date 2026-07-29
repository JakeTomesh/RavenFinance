using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RavenFinance.Data;
using RavenFinance.Models;

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
        public IActionResult ReturnToPaymentSource(int financeRecordId)
        {
            if(financeRecordId < 0)
            {
                TempData["ErrorMessage"] = "Invalide finance record.";
                return RedirectToAction("Index", "FinanceRecord");
            }
            try
            {
                var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

                FinanceRecord record = HelperUser.getFinanceRecordById(dbContext, financeRecordId, user.UserId);
                if(record == null)
                {
                    TempData["ErrorMessage"] = "Error retrieving finance record.";
                    return RedirectToAction("Index");
                }

                //build user PaymentSource and Saving into obj

                List<PaymentSource> userPaymentSource = HelperUser.GetPaymentSourcesByUserId(dbContext, user.UserId);

                List<Saving> userSavings = HelperUser.GetSavingsByUserId(dbContext, user.UserId);

                // Initialize PaymentSource if it's null
                if (record.PaymentSource == null)
                {
                    record.PaymentSource = new List<PaymentSource>();
                }

                // Initialize Saving if it's null
                if (record.Saving == null)
                {
                    record.Saving = new List<Saving>();
                }

                foreach (PaymentSource item in userPaymentSource)
                {
                    if (item != null)
                    {
                        record.PaymentSource.Add(item);

                    }
                }

                foreach (Saving item in userSavings)
                {
                    if (item != null)
                    {
                        record.Saving.Add(item);
                    }
                }

                return View("PaymentSource", record);
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

                    // Initialize PaymentSource if it's null
                    if(record.PaymentSource == null)
                    {
                        record.PaymentSource = new List<PaymentSource>();
                    }

                    // Initialize Saving if it's null
                    if (record.Saving == null)
                    {
                        record.Saving = new List<Saving>();
                    }

                    foreach (PaymentSource item in userPaymentSource)
                    {
                        if(item != null)
                        {
                            record.PaymentSource.Add(item);

                        }
                    }

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

                    return View("PaymentSource", record);
                }
                else
                {
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
                    DateOfTransaction = DateTime.Today,
                    IsExpense = true,
                    IsDeposit = false,
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
            //add statement items to payment source
            //return to payment source selection

        }

        [HttpGet]
        public IActionResult Saving(FinanceRecord record)
        {



            return View();
        }

        [HttpPost]
        public IActionResult CreateStatementItem(StatementItemCreateViewModel item, string submitAction)
        
        {

            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            if (item.DateOfTransaction > DateTime.Today)
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

            if(item.CategoryId == null)
            {
                ModelState.AddModelError(nameof(item.CategoryId), "Category selection is required.");
            }

            if(item.IsDeposit == false && item.IsExpense == false)
            {
                ModelState.AddModelError(nameof(item.IsExpense), "Expense or deposite must be declared.");
                ModelState.AddModelError(nameof(item.IsDeposit), "Expense or deposite must be declared.");
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

            


            return View();
        }
    }

    
}
