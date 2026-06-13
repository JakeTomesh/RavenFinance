namespace RavenFinance.Models
{
    public class RecurringBills
    {
        public int RecurringBillsId { get; set; }
        public required int UserId { get; set; }
        public required string Name { get; set; }
        public required decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
        public bool IsSplit { get; set; }
        public required PaymentSource PaymentSource { get; set; }
    }
}
