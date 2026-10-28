namespace RavenFinance.Models
{
    /// <summary>
    /// Everything shown on the ReviewFinanceRecord page: dates, statement items grouped by
    /// payment source, savings contributions, and the overall summary.
    /// </summary>
    public class ReviewFinanceRecordViewModel
    {
        public int FinanceRecordId { get; set; }
        public DateOnly DateStart { get; set; }
        public DateOnly DateEnd { get; set; }
        public bool IsComplete { get; set; }

        public List<ReviewPaymentSourceGroup> PaymentSourceGroups { get; set; } = new();
        public List<SavingContribution> Contributions { get; set; } = new();

        // Summary values (from HelperUser.CalculateGrossFinanceRecordValue)
        public decimal DepositTotal { get; set; }
        public decimal ExpenseTotal { get; set; }
        public decimal NetTotal { get; set; }
        public decimal SavingsTotal { get; set; }
        public decimal SavingsSpentTotal { get; set; }
        public decimal RemainingTotal { get; set; }

        // Gross amount per category (all items regardless of type) for categories used in this record.
        // Categories with no items or a zero total are excluded.
        public List<CategoryBalance> CategoryBalances => PaymentSourceGroups
            .SelectMany(g => g.Items)
            .GroupBy(i => i.Category?.Name ?? "Uncategorized")
            .Select(g => new CategoryBalance
            {
                CategoryName = g.Key,
                Balance = g.Sum(i => i.Amount)
            })
            .Where(c => c.Balance != 0)
            .OrderBy(c => c.CategoryName)
            .ToList();
    }

    public class CategoryBalance
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    // Statement items belonging to one payment source within the record
    public class ReviewPaymentSourceGroup
    {
        public int PaymentSourceId { get; set; }
        public string PaymentSourceName { get; set; } = string.Empty;
        public List<StatementItem> Items { get; set; } = new();

        public decimal Deposits => Items.Where(i => i.ItemType == StatementItem.StatementItemType.Deposit).Sum(i => i.Amount);
        public decimal Expenses => Items.Where(i => i.ItemType == StatementItem.StatementItemType.Expense).Sum(i => i.Amount);
    }
}
