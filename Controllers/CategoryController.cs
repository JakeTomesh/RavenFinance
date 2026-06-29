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

        [HttpGet]
        public IActionResult Delete(int categoryId)
        {
            try
            {
                User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                if (user == null)
                    return RedirectToAction("Index", "Category");

                Category? category = HelperUser.GetCategoryByCategoryIdAndUserId(dbContext, categoryId, user.UserId);
        
                if (category != null)
                {
                    return View(category); 
                }
                else
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                if (user == null)
                    return RedirectToAction("Index", "Category");

                Category? category = HelperUser.GetCategoryByCategoryIdAndUserId(dbContext, categoryId, user.UserId);
                if (category != null)
                {
                    HelperUser.DeleteCategory(dbContext, category);
                    TempData["SuccessMessage"] = "Category deleted successfully!";
                    return RedirectToAction("Index", "Category");
                }
                else
                {
                    TempData["ErrorMessage"] = "Category not found. Delete unsuccessful.";
                    return View();
                }
                
            }catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Index");
            }
            
        }

        [HttpGet]
        public IActionResult Edit(int categoryId)
        {
            if(categoryId > 0)
            {
                try
                {
                    User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "User not found.";
                        return RedirectToAction("Index", "Category");
                    }
                    Category? category = HelperUser.GetCategoryByCategoryIdAndUserId(dbContext, categoryId, user.UserId);
                    return View(category);
                }catch(Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
                
            }
            else
            {
                ViewBag.ErrorMessage = "Category not found";
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult EditCategory(int categoryId, string Name)
        {
            if(categoryId > 0)
            {
                try
                {
                    User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                    if(user == null)
                    {
                        TempData["ErrorMessage"] = "User not found.";
                        return RedirectToAction("Index", "Category");
                    }

                    Category? category = HelperUser.GetCategoryByCategoryIdAndUserId(dbContext, categoryId, user.UserId);

                    if(category != null)
                    {
                        //assign new name to category object
                        category.Name = Name;
                        HelperUser.UpdateCategory(dbContext, category);
                        TempData["SuccessMessage"] = "Category updated successfully!";
                        return RedirectToAction("Index", "Category");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Category not found.";
                        return View("Index");
                    }
                    
                }catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
            }else
            {
                ViewBag.ErrorMessage = "Category not found.";
                return View("Index");
            }
        }
    }
}
