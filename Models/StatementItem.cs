namespace RavenFinance.Models
{
    public class StatementItem
    {
        public int StatementItemId { get; set; }
        public string? Description { get; set; }
        public required decimal Amount { get; set; }
        public string? Merchant { get; set; }
        public required DateOnly DateOfTransaction { get; set; }
        public required bool IsExpense { get; set; }
        public required bool IsDeposit { get; set; }
        public bool IsMonthlyRecurringBill { get; set; }
        //set category with fk_id
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public required int UserId { get; set; }
        public int PaymentSourceId { get; set; }
        public PaymentSource? PaymentSource { get; set; }
    }
}
