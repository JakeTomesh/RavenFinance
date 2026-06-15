namespace RavenFinance.Models
{
    public class RecurringBill
    {
        public int RecurringBillId { get; set; }
        public required int UserId { get; set; }
        public required Category Category { get; set; }
        public required string Name { get; set; }
        public required decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
        public bool IsSplit { get; set; }
        public required PaymentSource PaymentSource { get; set; }

        public RecurringBill() { }

        public RecurringBill(int userId, string name, decimal amount, DateTime dueDate, Category category, PaymentSource paymentSource, string notes, bool isSplit)
        {
            userId = UserId;
            name = Name;
            amount = Amount;
            dueDate = DueDate;
            category = Category;
            paymentSource = PaymentSource;
            notes = Notes;
            isSplit = IsSplit;
        }
    }
}
