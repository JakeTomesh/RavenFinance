namespace RavenFinance.Models
{
    /// <summary>
    /// A single committed contribution to a <see cref="Saving"/> account, made while
    /// building a specific <see cref="FinanceRecord"/>. A Saving's balance is the sum of its contributions.
    /// </summary>
    public class SavingContribution
    {
        public int SavingContributionId { get; set; }

        public int SavingId { get; set; }
        public Saving? Saving { get; set; }

        public int FinanceRecordId { get; set; }
        public FinanceRecord? FinanceRecord { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public DateTime DateCommitted { get; set; } = DateTime.UtcNow;
    }
}
