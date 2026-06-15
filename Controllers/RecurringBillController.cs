using Microsoft.AspNetCore.Mvc;
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
            ViewBag.Categories = categories.ToList();
            return View();
        }


        [HttpPost]
        public IActionResult Create(string name, decimal amount, DateTime dueDate, Category category, PaymentSource paymentSource, string notes, bool isSplit)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            if(userId > 0)
            {
                RecurringBill bill = new RecurringBill
                {
                    UserId = userId,
                    Name = name,
                    Amount = amount,
                    DueDate = dueDate,
                    Category = category,
                    PaymentSource = paymentSource,
                    Notes = notes,
                    IsSplit = isSplit
                };
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
            return View();
        }
    }
}
