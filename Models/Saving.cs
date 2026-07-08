namespace RavenFinance.Models
{
    public class Saving
    {
        public int SavingId { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }
        public ICollection<StatementItem>? StatementItem { get; set; }
    }
}
