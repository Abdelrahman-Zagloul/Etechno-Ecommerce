namespace Ecommerce.Models.ViewModels
{
    public class UpdateCategoryViewModel
    {
        public int? CategoryId { get; set; }


        [Required(ErrorMessage = "Category Name Is Required")]
        [Display(Name = "Category Name")]
        [Length(1,2,ErrorMessage = "Length Of Category Name Must be Between 1 and 100 char")]
        public string CategoryName { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Display Order") ]
        [Range(1, 100,ErrorMessage = "Order Must be Between Range 1 and 100")]
        public int DisplayOrder { get; set; }


    }
}
