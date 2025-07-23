
namespace Ecommerce.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
        public decimal TotalPrice { get; set; }
    }
}
