namespace RavenFinance.Models
{
    public class SavingItem
    {
        public int SavingItemId { get; set; }
        public int SavingId { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required decimal Amount { get; set; }
        public DateOnly? DateOfDeposit { get; set; }
    }
}
