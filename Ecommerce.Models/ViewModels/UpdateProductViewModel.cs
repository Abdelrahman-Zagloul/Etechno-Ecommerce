
namespace Ecommerce.Models.ViewModels
{
    public class UpdateProductViewModel
    {
        [ValidateNever]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name must not exceed 100 characters")]
        public string Name { get; set; } = string.Empty;


        [StringLength(5000, ErrorMessage = "Description must not exceed 5000 characters")]
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN must not exceed 20 characters")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author name is required")]
        [StringLength(100, ErrorMessage = "Author name must not exceed 100 characters")]
        public string Author { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }

        public string? ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "List price is required")]
        [Range(1, 10000, ErrorMessage = "List price must be between 1 and 10000")]
        public decimal ListPrice { get; set; }

        [Required(ErrorMessage = "Price for 50 units is required")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10000")]
        public decimal Price50 { get; set; }

        [Required(ErrorMessage = "Price for 100 units is required")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10000")]
        public decimal Price100 { get; set; }

        [Required(ErrorMessage = "Category selection is required")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();
    }
}
