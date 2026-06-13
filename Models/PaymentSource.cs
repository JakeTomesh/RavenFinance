namespace RavenFinance.Models
{
    public class PaymentSource
    {
        public int PaymentSourceId { get; set; }
        public required int UserId { get; set; }
        public required string Name { get; set; }
        //statement item refers to the specific transaction on the statement that this payment source is associated with. 
        public StatementItem? StatementItem { get; set; }
        
    }
}
