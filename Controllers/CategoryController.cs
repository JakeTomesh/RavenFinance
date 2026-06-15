using Microsoft.AspNetCore.Mvc;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    public class CategoryController : Controller
    {
        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public CategoryController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database

        public IActionResult Index()
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            if(userId > 0)
            {
                try
                {
                    var userCategories = HelperUser.GetCategoriesByUserId(dbContext, userId);
                    return View(userCategories);
                }catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View();
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            if(userId > 0)
            {
                try
                {
                    Category category = new Category
                    {
                        UserId = userId,
                        Name = name
                    };

                    HelperUser.CreateNewCategory(dbContext, category);
                    TempData["SuccessMessage"] = "Category added successfully!";
                    return RedirectToAction("Index", "Category");

                }catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }
            return View("Index");
        }
    }
}
