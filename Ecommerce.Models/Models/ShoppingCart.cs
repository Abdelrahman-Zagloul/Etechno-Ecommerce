namespace Ecommerce.Models.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; } // Derived Attribute (Not mapped in DB)
        public Product? Product { get; set; }
        public string UserId { get; set; }
    }
}
