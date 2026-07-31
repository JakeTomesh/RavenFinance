namespace RavenFinance.Models
{
    public class FinanceRecordPaymentSourceViewModel
    {
        public int FinanceRecordId { get; set; }

        public ICollection<PaymentSourceSummaryViewModel> PaymentSources { get; set; } = new List<PaymentSourceSummaryViewModel>();

    }
}
