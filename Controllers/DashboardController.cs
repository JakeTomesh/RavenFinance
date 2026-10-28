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
            var user = HelperUser.ValidateCurrentUser(dbContext, HttpContext);
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Invalid user, please login to continue";
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // All of the user's records, including soft-deleted ones, split into completed / draft / deleted
                var records = dbContext.FinanceRecords
                    .Where(r => r.UserId == user.UserId)
                    .OrderByDescending(r => r.DateEnd)
                    .ToList();
                var ids = records.Select(r => r.FinanceRecordId).ToList();

                var items = dbContext.StatementItems
                    .Where(s => ids.Contains(s.FinanceRecordId))
                    .Select(s => new { s.FinanceRecordId, s.ItemType, s.Amount })
                    .ToList();
                var contributions = dbContext.SavingContributions
                    .Where(c => ids.Contains(c.FinanceRecordId))
                    .Select(c => new { c.FinanceRecordId, c.Amount })
                    .ToList();

                var summaries = records.Select(r => new FinanceRecordSummary
                {
                    FinanceRecordId = r.FinanceRecordId,
                    DateStart = r.DateStart,
                    DateEnd = r.DateEnd,
                    ItemCount = items.Count(i => i.FinanceRecordId == r.FinanceRecordId),
                    DepositTotal = items.Where(i => i.FinanceRecordId == r.FinanceRecordId && i.ItemType == StatementItem.StatementItemType.Deposit).Sum(i => i.Amount),
                    ExpenseTotal = items.Where(i => i.FinanceRecordId == r.FinanceRecordId && i.ItemType == StatementItem.StatementItemType.Expense).Sum(i => i.Amount),
                    SavingsTotal = contributions.Where(c => c.FinanceRecordId == r.FinanceRecordId).Sum(c => c.Amount),
                    IsComplete = r.IsComplete,
                    IsDeleted = r.IsDeleted
                }).ToList();

                var model = new DashboardViewModel
                {
                    Records = summaries.Where(s => !s.IsDeleted && s.IsComplete).ToList(),
                    Drafts = summaries.Where(s => !s.IsDeleted && !s.IsComplete).ToList(),
                    Deleted = summaries.Where(s => s.IsDeleted).ToList(),
                    Tasks = dbContext.TaskItems.Where(t => t.UserId == user.UserId).OrderBy(t => t.Position).ToList()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while loading your finance history: " + ex.Message;
                return View(new DashboardViewModel());
            }
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
