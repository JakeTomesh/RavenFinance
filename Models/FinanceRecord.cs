namespace RavenFinance.Models
{
    public class FinanceRecord
    {
        public int FinanceRecordId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        //user selects date range
        public DateOnly DateStart { get; set; }
        public DateOnly DateEnd { get; set; }

        public bool IsComplete { get; set; } = false;
        
        //user selects payment source and creates new statement items for income & expenses
        public required ICollection<StatementItem> StatementItems= new List<StatementItem>();

        public ICollection<Saving> Saving = new List<Saving>();
        
    }
}
