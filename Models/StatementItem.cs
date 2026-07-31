namespace RavenFinance.Models
{
    public class StatementItem
    {
        public int StatementItemId { get; set; }

        public string? Description { get; set; }
        
        public decimal Amount { get; set; }
       
        public string? Merchant { get; set; }
        
        public DateOnly DateOfTransaction { get; set; }
        
        public enum StatementItemType
        {
            Deposit = 0,
            Expense = 1
        }
        public StatementItemType ItemType { get; set; }
        
        public bool IsMonthlyRecurringBill { get; set; }
        
        //set category with fk_id
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int PaymentSourceId { get; set; }
        public PaymentSource? PaymentSource { get; set; }

        public int FinanceRecordId { get; set; }
        public FinanceRecord? FinanceRecord { get; set; }
    }
}
