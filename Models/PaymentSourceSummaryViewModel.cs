namespace RavenFinance.Models
{
    public class PaymentSourceSummaryViewModel
    {
        public int PaymentSourceId { get; set; }

        public required string Name { get; set; }

        public bool IsActive { get; set; }

        public int StatementItemCount { get; set; } = 0;
    }
}
