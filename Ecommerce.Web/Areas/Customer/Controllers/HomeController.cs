
namespace Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string? CategoryName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.UserId == userId).Count();
                HttpContext.Session.SetInt32(SD.CardNumber, count);
            }

            var allCategory = _unitOfWork.Categories.GetAll().ToList();

            IEnumerable<Product> allProduct;
            if (!string.IsNullOrEmpty(CategoryName))
            {
                allProduct = _unitOfWork.Products.GetAll(x => x.Category.CategoryName == CategoryName, nameof(Category));
            }
            else
            {
                allProduct = _unitOfWork.Products.GetAll(null, nameof(Category));
            }

            var productViewModels = allProduct.Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Author = x.Author,
                ImageUrl = x.ImageUrl,
                ListPrice = x.ListPrice,
                Price100 = x.Price100
            }).ToList();

            var homePageViewModel = new HomePageViewModel
            {
                Products = productViewModels,
                AllCategories = allCategory
            };

            return View(homePageViewModel);
        }


        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.Products.Get(x => x.Id == productId, nameof(Category));
            return View(product);
        }
        [Authorize]
        public IActionResult AddToCart(int productId, int count)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var shoppingCart = new ShoppingCart
            {
                ProductId = productId,
                UserId = userId,
                Count = count
            };
            var existingCartItem = _unitOfWork.ShoppingCart.Get(
                x => x.UserId == userId && x.ProductId == shoppingCart.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(existingCartItem);
                TempData["success"] = $"+{count} Added Successfully.";
                TempData["success1"] = $"Updated item quantity to {existingCartItem.Count}.";

            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                TempData["success"] = $"Added {shoppingCart.Count} item(s) to cart.";
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            TempData["error"] = "Unexpected Error.";
            return View();
        }
    }
}
