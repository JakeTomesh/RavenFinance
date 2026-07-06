namespace RavenFinance.Models
{
    public class StatementItem
    {
        public int StatementItemId { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
        public required string Merchant { get; set; }
        public required bool isExpense { get; set; }
        public required bool isDeposit { get; set; }
        
        public bool? IsMonthlyRecurringBill { get; set; }
        public int CategoryId { get; set; }
        public required int UserId { get; set; }

    }
}
