using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public IActionResult DateSelection(FinaceRecord record)
        {
            if (ModelState.IsValid)
            {
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

        [HttpPost]
        public IActionResult PaymentSource(FinaceRecord record)
        {
            return View(record);
        }
    }

    
}
