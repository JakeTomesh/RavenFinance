using System.Text.Json.Nodes;

namespace RavenFinance.Models
{
    public class SavingsViewModel
    {
        // Finance record being built; posted back with the contribution
        public int FinanceRecordId { get; set; }
        public int UserId { get; set; }

        // Savings account chosen in the select element (posted back)
        public int SelectedSavingId { get; set; }

        // Amount the user wants to commit to the selected savings account (posted back)
        public decimal ContributionAmount { get; set; }

        // All savings accounts belonging to the user; populates the select element
        public List<Saving> SavingsOptions { get; set; } = new();

        // Totals from HelperUser.CalculateGrossFinanceRecordValue (depositTotal, expenseTotal, netTotal)
        public JsonObject? GrossFinanceRecordValue { get; set; }
    }
}
