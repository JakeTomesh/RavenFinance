namespace RavenFinance.Models
{
    /// <summary>Data for the dashboard: the user's completed finance record history.</summary>
    public class DashboardViewModel
    {
        public List<FinanceRecordSummary> Records { get; set; } = new();

        // Incomplete records the user saved as a draft (shown in yellow)
        public List<FinanceRecordSummary> Drafts { get; set; } = new();

        // Soft-deleted records (kept in the database, shown as "deleted")
        public List<FinanceRecordSummary> Deleted { get; set; } = new();

        // The user's open to-do notes, in display order
        public List<TaskItem> Tasks { get; set; } = new();

        // Aggregates across completed (non-deleted) records only
        public decimal TotalDeposits => Records.Sum(r => r.DepositTotal);
        public decimal TotalExpenses => Records.Sum(r => r.ExpenseTotal);
        public decimal TotalSaved => Records.Sum(r => r.SavingsTotal);
    }

    /// <summary>One row in the history list.</summary>
    public class FinanceRecordSummary
    {
        public int FinanceRecordId { get; set; }
        public DateOnly DateStart { get; set; }
        public DateOnly DateEnd { get; set; }
        public int ItemCount { get; set; }
        public bool IsComplete { get; set; }
        public bool IsDeleted { get; set; }
        public decimal DepositTotal { get; set; }
        public decimal ExpenseTotal { get; set; }
        public decimal SavingsTotal { get; set; }
        public decimal NetTotal => DepositTotal - ExpenseTotal;
        public decimal RemainingTotal => NetTotal - SavingsTotal;
    }
}
