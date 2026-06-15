using Microsoft.AspNetCore.Mvc;
using RavenFinance.Data;
using RavenFinance.Models;
using System.Diagnostics;

namespace RavenFinance.Controllers
{
    public class LoginController : Controller
    {
        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public LoginController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            //validate user credentials
            var user = HelperUser.IsValidLogin(dbContext, Email, Password);
            if (user != null)
            {
                //if valid, set session and redirect to dashboard
                HttpContext.Session.SetInt32("UserId", user.UserId);
                ViewBag.UserMessage = " Welcome, " + user.FirstName;
                return RedirectToAction("Index", "Dashboard");

            }else
            {
                //if invalid, return to login page with error message\
                ViewBag.ErrorMessage = "Invalid email or password. Please try again.";
                return View("Index");
            }

        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string Email, string Password, string FirstName, string LastName)
        {
            //validate user credentials
            //try catch for validation errors and show to user
            try
            {
                var user = HelperUser.ValidateRegister(dbContext, Email, Password, FirstName, LastName);

                if (user != null)
                {
                    //if valid, return to login page with success message
                    ViewBag.SuccessMessage = "Registration successful. Please log in.";
                    return View("Index");
                }
                else
                {
                    //if invalid, return to register page with error message
                    ViewBag.ErrorMessage = "Email already exists. Please try again.";
                    return View("Register");
                }
            }
            catch (Exception ex)
            {
                //if invalid, return to register page with error message
                ViewBag.ErrorMessage = ex.Message;
                return View("Register");
            }
            
            
        }

        public IActionResult Logout()
        {
            // Clear the session to log out the user
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
