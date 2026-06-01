namespace RavenFinance.Models
{
    public class MonthlyBills
    {
        public required string Name { get; set; }
        public required decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
    }
}
