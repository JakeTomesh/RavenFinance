namespace RavenFinance.Models
{
    //Any type of credit, bank, or source of $$$. 
    public class PaymentSource
    {
        public int PaymentSourceId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public required string Name { get; set; }

        public bool IsActive { get; set; } = true;

        //statement item refers to the specific transaction on the statement that this payment source is associated with. 
        public ICollection<StatementItem> StatementItems = new List<StatementItem>();
        
    }
}
