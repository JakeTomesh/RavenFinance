using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    public class UserController : Controller
    {

        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public UserController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database


        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if(userId != null)
            {
                var user = HelperUser.GetUserById(dbContext, userId.Value);
                return View("Profile", user);
            }
            return View("Profile");
        }


        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                try
                {
                    var user = HelperUser.GetUserById(dbContext, userId.Value);
                    return View(user);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while retrieving the user profile: " + ex.Message;
                    return View("Profile");
                }

            }
            ViewBag.ErrorMessage = "User profile not found.";
            return View("Profile");
        }

        public IActionResult EditProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                try
                {
                    var user = HelperUser.GetUserById(dbContext, userId.Value);
                    return View("Edit", user);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while retrieving the user profile: " + ex.Message;
                    return View("Profile");
                }
            }
            ViewBag.ErrorMessage = "User profile not found.";
            return View("Profile");
        }

        public IActionResult UpdateProfile(int id, string FirstName, string LastName, string Email, string Password)
        {
            try
            {
                var user = HelperUser.GetUserById(dbContext, id);
                if (user != null)
                {
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.Email = Email;
                    if(Password != null)
                    {
                        user.Password = Password;
                    }
                    dbContext.Users.Update(user);
                    dbContext.SaveChanges();
                    ViewBag.SuccessMessage = "Profile updated successfully.";
                    return View("Profile");
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View("Profile");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the profile: " + ex.Message;
                return View("Profile");
            }
        }
    }
}
