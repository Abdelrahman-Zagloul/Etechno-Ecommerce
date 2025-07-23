namespace Ecommerce.Models.ViewModels
{
    public class OrderHeaderViewModel
    {
        [Required(ErrorMessage = "The Name is Required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Phone Number is Required.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(01[0125])[0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "The Street Address is Required.")]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; } = string.Empty;


        [Required(ErrorMessage = "The City is Required.")]
        public string City { get; set; } = string.Empty;

        public IEnumerable<ShoppingCart>? ShoppingCarts { get; set; }
        public decimal? TotalPrice { get; set; }

    }
}
