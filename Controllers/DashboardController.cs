using Microsoft.AspNetCore.Mvc;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    public class DashboardController : Controller
    {

        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public DashboardController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId != null)
            {
                try
                {
                    var user = HelperUser.GetUserById(dbContext, userId.Value);
                    return View(userId);
                }catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while retrieving the user profile: " + ex.Message;
                    return View("Index");
                }
                
            }
            ViewBag.ErrorMessage = "User profile not found.";
            return View("Index");
        }

        [HttpGet]
        public IActionResult RecurringBills()
        {
            //gather monthly bill info from database and pass to view
            var userId = HelperUser.GetUserFromSession(HttpContext);
            if(userId > 0)
            {
                try
                {
                    var userRecurringBills = HelperUser.GetRecurringBillsByUserId(dbContext, userId);
                    return View("RecurringBills", userRecurringBills);
                }catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while retrieving the monthly bills: " + ex.Message;
                    return View("Index");
                }
            }
            ViewBag.ErrorMessage = "User not found.";
            return View("Index");
        }
    }
}
