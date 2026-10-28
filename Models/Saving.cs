namespace RavenFinance.Models
{
    public class Saving
    {
        public int SavingId { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }

        // Initial amount set when the savings is created; contributions and expenses adjust the balance from here
        [System.ComponentModel.DataAnnotations.Range(0, 1000000000, ErrorMessage = "Initial amount cannot be negative.")]
        public decimal Amount { get; set; }
        public ICollection<StatementItem>? StatementItem { get; set; }

        // Every contribution committed to this savings account
        public ICollection<SavingContribution> Contributions { get; set; } = new List<SavingContribution>();

        // Overall balance: contributions minus expenses paid from this account. Computed, not stored.
        // Contributions and StatementItem must be loaded for this to be accurate.
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal Balance => Amount + Contributions.Sum(c => c.Amount)
            - (StatementItem?.Where(s => s.ItemType == Models.StatementItem.StatementItemType.Expense).Sum(s => s.Amount) ?? 0m);
    }
}
