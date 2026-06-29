using Microsoft.AspNetCore.Mvc;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    public class PaymentSourceController : Controller
    {
        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public PaymentSourceController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database

        public IActionResult Index()
        {
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if(user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Login");
            }
            List<PaymentSource> paymentSources = HelperUser.GetPaymentSourcesByUserId(dbContext, user.UserId);
            return View(paymentSources);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PaymentSource paymentSource)
        {
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
                return RedirectToAction("Index", "Login");

            if(!ModelState.IsValid)
                return View(paymentSource);

            try
            {
                paymentSource.UserId = user.UserId;
                HelperUser.CreateNewPaymentSource(dbContext, paymentSource);
                TempData["SuccessMessage"] = "Payment source added successfully!";
                return RedirectToAction("Index", "PaymentSource");
            }catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }

        }
    }
}
