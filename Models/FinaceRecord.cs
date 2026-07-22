namespace RavenFinance.Models
{
    public class FinaceRecord
    {
        public int FinanceRecordId { get; set; }
        public int UserId { get; set; }
        //user selects date range
        public DateOnly DateStart { get; set; }
        public DateOnly DateEnd { get; set; }
        //user selects payment source and creates new statement items for income & expenses
        public required ICollection<PaymentSource> PaymentSource = new List<PaymentSource>();
        public ICollection<Saving>? Saving { get; set; }
        
    }
}
