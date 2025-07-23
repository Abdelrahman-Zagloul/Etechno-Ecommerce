
namespace Ecommerce.Models.ViewModels
{
    public class OrderViewModel
    {
        [ValidateNever]
        public OrderHeader Header { get; set; }
        [Required(ErrorMessage = "Carrier is required when shipping.")]
        public string Carrier => Header?.Carrier;
        [ValidateNever]
        public List<OrderDetail> Details { get; set; }
        [ValidateNever]
        public string Email { get; set; }
        [ValidateNever]
        public decimal TotalPrice { get; set; }
    }
}
