using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    public class RecurringBillController : Controller
    {

        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public RecurringBillController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database
        
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            var categories = HelperUser.GetCategoriesByUserId(dbContext, userId);
            var paymentSources = HelperUser.GetPaymentSourcesByUserId(dbContext,userId);
            
            ViewBag.Categories = categories;
            ViewBag.PaymentSources = paymentSources;
            return View();
        }


        [HttpPost]
        public IActionResult Create(RecurringBill recurringBill)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if(userId > 0)
            {
                if (ModelState.IsValid)
                {
                    RecurringBill bill = new RecurringBill
                    {
                        UserId = userId,
                        Name = recurringBill.Name,
                        Amount = recurringBill.Amount,
                        DueDate = recurringBill.DueDate,
                        CategoryId = recurringBill.CategoryId,
                        //Category = recurringBill.Category,
                        PaymentSourceId = recurringBill.PaymentSourceId,
                        //PaymentSource = recurringBill.PaymentSource,
                        Notes = recurringBill.Notes,
                        IsSplit = recurringBill.IsSplit
                    };
                    //use category id to populate category object into constructor
                    //same for payment source
                    Category? category = HelperUser.GetCategoryByCategoryIdAndUserId(dbContext, bill.CategoryId, userId);

                    PaymentSource? paymentSource = HelperUser.GetPaymentSourceById(dbContext, bill.PaymentSourceId, userId);
                    //add objects to constructor
                    bill.PaymentSource = paymentSource;
                    bill.Category = category;

                    //db call
                    try
                    {
                        HelperUser.CreateNewRecurringBill(dbContext, bill);
                        TempData["SuccessMessage"] = "New recurring bill added seccessfully!";
                        return RedirectToAction("RecurringBills", "Dashboard");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "An error occured while saving new recurring bill. " + ex;

                        return View("Create");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Error constructing new recurring bill.";
                    return RedirectToAction("Create", recurringBill);
                }

            }
            else
            {
                ViewBag.ErrorMessage = "User not found.";
                return View();

            }
        }
    }
}
