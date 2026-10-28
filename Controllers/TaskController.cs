using Microsoft.AspNetCore.Mvc;
using RavenFinance.Data;
using RavenFinance.Models;

namespace RavenFinance.Controllers
{
    /// <summary>
    /// JSON endpoints behind the dashboard task list. Every query is scoped to the
    /// logged-in user, so one user can never read or change another user's tasks.
    /// </summary>
    public class TaskController : Controller
    {
        private const int MaxTextLength = 200;
        private readonly RavenDbContext dbContext;

        public TaskController(RavenDbContext db)
        {
            dbContext = db;
        }

        /// <summary>Adds a task to the end of the list and returns its new id.</summary>
        [HttpPost]
        public IActionResult Add(string text)
        {
            if (CurrentUserId() is not int userId) return Unauthorized();
            if (!TryCleanText(text, out string clean)) return BadRequest();

            int nextPosition = dbContext.TaskItems.Where(t => t.UserId == userId)
                .Max(t => (int?)t.Position + 1) ?? 0;
            var task = new TaskItem { UserId = userId, Text = clean, Position = nextPosition };
            dbContext.TaskItems.Add(task);
            dbContext.SaveChanges();
            return Json(new { id = task.TaskItemId });
        }

        /// <summary>Changes the text of an existing task.</summary>
        [HttpPost]
        public IActionResult Edit(int id, string text)
        {
            if (CurrentUserId() is not int userId) return Unauthorized();
            if (!TryCleanText(text, out string clean)) return BadRequest();

            TaskItem? task = dbContext.TaskItems.FirstOrDefault(t => t.TaskItemId == id && t.UserId == userId);
            if (task == null) return NotFound();

            task.Text = clean;
            dbContext.SaveChanges();
            return Ok();
        }

        /// <summary>Marking a task complete deletes it.</summary>
        [HttpPost]
        public IActionResult Complete(int id)
        {
            if (CurrentUserId() is not int userId) return Unauthorized();

            TaskItem? task = dbContext.TaskItems.FirstOrDefault(t => t.TaskItemId == id && t.UserId == userId);
            if (task == null) return NotFound();

            dbContext.TaskItems.Remove(task);
            dbContext.SaveChanges();
            return Ok();
        }

        /// <summary>Saves a new order. <paramref name="ids"/> is every task id in its new top-to-bottom order.</summary>
        [HttpPost]
        public IActionResult Reorder(int[] ids)
        {
            if (CurrentUserId() is not int userId) return Unauthorized();

            var tasks = dbContext.TaskItems.Where(t => t.UserId == userId).ToList();
            for (int position = 0; position < ids.Length; position++)
            {
                TaskItem? task = tasks.FirstOrDefault(t => t.TaskItemId == ids[position]);
                if (task != null) task.Position = position;
            }
            dbContext.SaveChanges();
            return Ok();
        }

        /// <summary>The logged-in user's id from the session, or null when not logged in.</summary>
        private int? CurrentUserId() => HttpContext.Session.GetInt32("UserId");

        /// <summary>Trims the text and rejects empty or over-long values.</summary>
        private static bool TryCleanText(string? text, out string clean)
        {
            clean = (text ?? string.Empty).Trim();
            return clean.Length > 0 && clean.Length <= MaxTextLength;
        }
    }
}
