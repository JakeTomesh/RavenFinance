namespace RavenFinance.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public required int UserId { get; set; }
        public required string Name { get; set; }
        public decimal? Amount { get; set; }

    }
}
