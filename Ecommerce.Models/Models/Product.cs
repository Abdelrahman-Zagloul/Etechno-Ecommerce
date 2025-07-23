namespace Ecommerce.Models.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public string? ImageUrl { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Price50 { get; set; }
        public decimal Price100 { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
