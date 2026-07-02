using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    public class RecurringBillController : Controller
    {

        // Inject the RavenDbContext to access the database
        private RavenDbContext dbContext { get; set; }

        public RecurringBillController(RavenDbContext db)
        {
            dbContext = db;
        }
        //must pass in dbContext to the HelperUser class to use database

        [HttpGet]
        public IActionResult Index()
        {
            //gather monthly bill info from database and pass to view
            var userId = HelperUser.GetUserFromSession(HttpContext);
            if (userId > 0)
            {
                try
                {
                    var userRecurringBills = HelperUser.GetRecurringBillsByUserId(dbContext, userId);
                    return View("Index", userRecurringBills);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while retrieving the monthly bills: " + ex.Message;
                    return View("Index");
                }
            }
            ViewBag.ErrorMessage = "User not found.";
            return RedirectToAction("Logout", "Login");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            var categories = HelperUser.GetCategoriesByUserId(dbContext, userId);
            var paymentSources = HelperUser.GetPaymentSourcesByUserId(dbContext,userId);
            
            ViewBag.Categories = categories;
            ViewBag.PaymentSources = paymentSources;
            return View();
        }


        [HttpPost]
        public IActionResult Create(RecurringBill recurringBill)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if(userId > 0)
            {
                if (ModelState.IsValid)
                {
                    RecurringBill bill = new RecurringBill
                    {
                        UserId = userId,
                        Name = recurringBill.Name,
                        Amount = recurringBill.Amount,
                        DueDate = recurringBill.DueDate,
                        CategoryId = recurringBill.CategoryId,
                        //Category = recurringBill.Category,
                        PaymentSourceId = recurringBill.PaymentSourceId,
                        //PaymentSource = recurringBill.PaymentSource,
                        Notes = recurringBill.Notes,
                        IsSplit = recurringBill.IsSplit
                    };
                    //use category id to populate category object into constructor
                    //same for payment source
                    Category? category = HelperUser.GetCategoryByCategoryIdAndUserId(dbContext, bill.CategoryId, userId);

                    PaymentSource? paymentSource = HelperUser.GetPaymentSourceById(dbContext, bill.PaymentSourceId, userId);
                    //add objects to constructor
                    bill.PaymentSource = paymentSource;
                    bill.Category = category;

                    //db call
                    try
                    {
                        HelperUser.CreateNewRecurringBill(dbContext, bill);
                        TempData["SuccessMessage"] = "New recurring bill added seccessfully!";
                        return RedirectToAction("Index", "RecurringBill");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "An error occured while saving new recurring bill. " + ex;

                        return View("Create");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Error constructing new recurring bill.";
                    return RedirectToAction("Create", recurringBill);
                }

            }
            else
            {
                ViewBag.ErrorMessage = "User not found.";
                return RedirectToAction("Logout", "Login");

            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not logged in. Please login to continue.";
               // HelperUser.DestroyUserSession(HttpContext);
                return RedirectToAction("Index", "Login");
            }
            if(id > 0)
            {
                try
                {
                    RecurringBill bill = HelperUser.GetRecurringBillByBillId(dbContext, id, user.UserId);

                    //populate select options for category and paymentsource
                    var categories = HelperUser.GetCategoriesByUserId(dbContext, userId);
                    var paymentSources = HelperUser.GetPaymentSourcesByUserId(dbContext, userId);

                    ViewBag.Categories = categories;
                    ViewBag.PaymentSources = paymentSources;

                    if (bill == null)
                    {
                        ViewBag.ErrorMessage = "Bill not found.";
                        return View("Index");
                    }else
                    {
                        return View(bill);
                    }
                }catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Index", "RecurringBill");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bill not found.";
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult EditRecurringBill(RecurringBill recurringBill)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if (userId > 0)
            {
                if (ModelState.IsValid)
                {

                    //use category id to populate category object into constructor
                    //same for payment source
                    Category? category = HelperUser.GetCategoryByCategoryIdAndUserId(dbContext, recurringBill.CategoryId, userId);

                    PaymentSource? paymentSource = HelperUser.GetPaymentSourceById(dbContext, recurringBill.PaymentSourceId, userId);
                    //add objects to constructor
                    recurringBill.PaymentSource = paymentSource;
                    recurringBill.Category = category;
                    recurringBill.UserId = user.UserId;

                    //db call
                    try
                    {
                        
                        HelperUser.UpdateRecurringBill(dbContext, recurringBill);
                        TempData["SuccessMessage"] = "Recurring bill updated seccessfully!";
                        return RedirectToAction("Index", "RecurringBill");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "An error occured while updating recurring bill. " + ex;

                        return View("Index");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Error constructing updated recurring bill.";
                    return RedirectToAction("Edit", recurringBill.RecurringBillId);
                }

            }
            else
            {
                ViewBag.ErrorMessage = "User not found.";
                return RedirectToAction("Logout", "Login");

            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if (id > 0 && user.UserId > 0)
            {
                try
                {
                    RecurringBill bill = HelperUser.GetRecurringBillByBillId(dbContext, id, user.UserId);
                    if(bill != null)
                    {
                        return View(bill);
                    }else
                    {
                        ViewBag.ErrorMessage = "No bill found.";
                        return View("Index");
                    }
                }catch(Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Logout", "Login");
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if (id > 0 && user.UserId > 0)
            {
                try
                {
                    RecurringBill bill = HelperUser.GetRecurringBillByBillId(dbContext, id, user.UserId);
                    return View(bill);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Logout", "Login");
            }
        }

        [HttpPost]
        public IActionResult DeleteRecurringBill(int recurringBillId)
        {
            var userId = HelperUser.GetUserFromSession(HttpContext);
            User? user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);

            if (user != null && user.UserId > 0)
            {
                if(recurringBillId > 0)
                {
                    try
                    {
                        HelperUser.DeleteRecurringBill(dbContext, recurringBillId, user.UserId);
                        TempData["SuccessMessage"] = "Recurring bill deleted successfully!";
                        return RedirectToAction("Index", "RecurringBill");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.Message;
                        return View("Index");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Bill not found.";
                    return View("Index");
                }
                
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Logout", "Login");
            }
        }
    }
}
