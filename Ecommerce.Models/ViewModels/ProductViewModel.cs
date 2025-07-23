namespace Ecommerce.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string? ImageUrl { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Price100 { get; set; }
    }
}
