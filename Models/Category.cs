namespace RavenFinance.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public required int UserId { get; set; }
        public required string Name { get; set; }

        public Category()
        {

        }

        public Category(int userId, string name)
        {
            userId = UserId;
            name = Name;
        }
    }
}
