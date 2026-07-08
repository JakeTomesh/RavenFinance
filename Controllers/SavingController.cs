using Microsoft.AspNetCore.Mvc;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    public class SavingController : Controller
    {

        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public SavingController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database
        public IActionResult Index()
        {
            
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if(user != null)
            {
                try
                {
                    List<Saving> savings = HelperUser.GetSavingsByUserId(dbContext, user.UserId);
                    return View(savings);
                }catch(Exception ex)
                {
                    TempData["ErrorMessage"] = ex;
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found. Please login to continue.";
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Saving saving)
        {
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        saving.UserId = user.UserId;

                        HelperUser.CreateNewSaving(dbContext, saving);
                        TempData["SuccessMessage"] = "Savings added successfully!";
                        return RedirectToAction("Index", "Saving");

                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                        return View("Index");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid savings details.";
                    return View("Create");
                }
                
            }else
            {
                TempData["ErrorMessage"] = "User not found. Please login to continue.";
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public IActionResult Delete(int savingId)
        {
            try
            {
                User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                if (user == null)
                    return RedirectToAction("Index", "Login");

                Saving? saving = HelperUser.GetSavingById(dbContext, savingId, user.UserId);

                if (saving != null)
                {
                    return View(saving);
                }
                else
                {
                    TempData["ErrorMessage"] = "Saving not found.";
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
        public IActionResult DeleteSaving(int savingId)
        {
            try
            {
                User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                if (user == null)
                    return RedirectToAction("Index", "Login");

                Saving? saving = HelperUser.GetSavingById(dbContext, savingId, user.UserId);
                if (saving != null)
                {
                    HelperUser.DeleteSaving(dbContext, saving);
                    TempData["SuccessMessage"] = "Saving deleted successfully!";
                    return RedirectToAction("Index", "Saving");
                }
                else
                {
                    TempData["ErrorMessage"] = "Saving not found. Delete unsuccessful.";
                    return View("Index");
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Index");
            }

        }


        [HttpGet]
        public IActionResult Edit(int savingId)
        {
            if (savingId > 0)
            {
                try
                {
                    User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "User not found.";
                        return RedirectToAction("Index", "Login");
                    }
                    Saving? saving = HelperUser.GetSavingById(dbContext, savingId, user.UserId);
                    return View(saving);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Saving not found";
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult EditSaving(Saving saving)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "User not found.";
                        return RedirectToAction("Index", "Login");
                    }

                    Saving? savingToBeUpdated= HelperUser.GetSavingById(dbContext, saving.SavingId, user.UserId);

                    if (savingToBeUpdated != null)
                    {
                        //assign new name to category object
                        savingToBeUpdated.Name = saving.Name;
                        HelperUser.UpdateSaving(dbContext, savingToBeUpdated);
                        TempData["SuccessMessage"] = "Saving updated successfully!";
                        return RedirectToAction("Index", "Saving");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Saving not found.";
                        return View("Index");
                    }

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Savings information not valid.";
                return View("Edit", saving.SavingId);
            }
        }
    }
}
