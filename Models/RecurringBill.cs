namespace RavenFinance.Models
{
    public class RecurringBill
    {
        public int RecurringBillId { get; set; }
        public required int UserId { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public required string Name { get; set; }
        public required decimal Amount { get; set; }
        public int DueDate { get; set; }
        public string? Notes { get; set; }

        // Expense bills are added to finance records as expenses, deposit bills as deposits
        public StatementItem.StatementItemType ItemType { get; set; } = StatementItem.StatementItemType.Expense;
        public int PaymentSourceId { get; set; }
        public PaymentSource? PaymentSource { get; set; }

        public RecurringBill() { }

        public RecurringBill(int userId, string name, decimal amount, int dueDate, Category category, PaymentSource paymentSource, string notes)
        {
            UserId = userId;
            Name = name;
            Amount = amount;
            DueDate = dueDate;
            Category = category;
            PaymentSource = paymentSource;
            Notes = notes;
        }
    }
}
