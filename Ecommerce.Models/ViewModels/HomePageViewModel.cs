namespace Ecommerce.Models.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; }
        public List<Category> AllCategories { get; set; }
    }

}
