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
            else
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }
            
        }

       
    }
}
