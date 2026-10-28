namespace RavenFinance.Models
{
    /// <summary>
    /// A short note/to-do on a user's dashboard task list. Stored in the "Task" table.
    /// Completed tasks are deleted, so every row is an open task.
    /// </summary>
    public class TaskItem
    {
        public int TaskItemId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public required string Text { get; set; }

        /// <summary>Zero-based display order within the user's list (changed by drag and drop).</summary>
        public int Position { get; set; }
    }
}
