namespace RavenFinance.Models
{
    public class FinanceRecordPaymentSourceViewModel
    {
        public int FinanceRecordId { get; set; }

        public ICollection<PaymentSourceSummaryViewModel> PaymentSources { get; set; } = new List<PaymentSourceSummaryViewModel>();

        // Savings accounts selectable as expense-only payment sources
        public ICollection<SavingSourceSummaryViewModel> SavingSources { get; set; } = new List<SavingSourceSummaryViewModel>();

    }

    public class SavingSourceSummaryViewModel
    {
        public int SavingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int StatementItemCount { get; set; }
    }
}
