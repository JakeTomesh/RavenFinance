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

        [HttpGet]
        public IActionResult Edit(int paymentSourceId)
        {
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                PaymentSource paymentSource = HelperUser.GetPaymentSourceById(dbContext, paymentSourceId, user.UserId);
                return View(paymentSource);
            }
            else
            {
                ViewBag.ErrorMessage = "No payment source found.";
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult EditPaymentSource(PaymentSource paymentSource)
        {
            
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //must update fields for hidden object properties
                    paymentSource.UserId = user.UserId;
                    paymentSource.Name = paymentSource.Name;
                    HelperUser.UpdatePaymentSource(dbContext, paymentSource);
                    TempData["SuccessMessage"] = "Payment source updated successfully!";
                    return RedirectToAction("Index", "PaymentSource");
                }catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Index", "PaymentSource");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid payment source model";
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult Delete(int paymentSourceId)
        {
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    PaymentSource paymentSource = HelperUser.GetPaymentSourceById(dbContext, paymentSourceId, user.UserId);
                    return View(paymentSource);
                    
                }catch(Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
            }else
            {
                ViewBag.ErrorMessage = "There was an error retrieving the payment source.";
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult DeletePaymentSource(int paymentSourceId)
        {

            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    PaymentSource paymentSource = HelperUser.GetPaymentSourceById(dbContext, paymentSourceId, user.UserId);
                    HelperUser.DeletePaymentSource(dbContext, paymentSource);
                    TempData["SuccessMessage"] = "Payment source deleted successfully!";
                    return RedirectToAction("Index", "PaymentSource");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "There was an error retrieving the payment source.";
                return View("Index");
            }
            
        }
    }
}
