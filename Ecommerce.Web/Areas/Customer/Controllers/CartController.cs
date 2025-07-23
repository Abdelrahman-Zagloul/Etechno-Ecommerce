
namespace Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            string appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.UserId == appUserId, nameof(Product));

            var shoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCarts = shoppingCarts,
            };
            foreach (var cart in shoppingCartViewModel.ShoppingCarts)
            {
                cart.Price = CalculatePriceBeasdOnCartNumber(cart);
                shoppingCartViewModel.TotalPrice += (cart.Price * cart.Count);
            }

            HttpContext.Session.SetInt32(SD.CardNumber, shoppingCarts.Count());
            return View(nameof(Index), shoppingCartViewModel);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            string appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(
                x => x.UserId == appUserId,
                nameof(Product)
            );

            var orderHeaderViewModel = new OrderHeaderViewModel { ShoppingCarts = shoppingCarts };
            decimal totalPrice = 0;
            foreach (var cart in orderHeaderViewModel.ShoppingCarts)
            {
                cart.Price = CalculatePriceBeasdOnCartNumber(cart);
                totalPrice += (cart.Price * cart.Count);
            }
            orderHeaderViewModel.TotalPrice = totalPrice;

            var infoForUser = _unitOfWork.OrderHeaders.Get(x => x.ApplicationUserId == appUserId);
            if (infoForUser != null)
            {
                orderHeaderViewModel.Name = infoForUser.Name;
                orderHeaderViewModel.StreetAddress = infoForUser.StreetAddress;
                orderHeaderViewModel.PhoneNumber = infoForUser.PhoneNumber;
                orderHeaderViewModel.City = infoForUser.City;
            }
            return View(orderHeaderViewModel);
        }

        [HttpPost]
        public IActionResult MakeOrder(OrderHeaderViewModel orderHeaderViewModel)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Summary));
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            // add new order header
            var newOrderHeader = new OrderHeader
            {
                ApplicationUserId = appUserId,
                City = orderHeaderViewModel.City,
                Name = orderHeaderViewModel.Name,
                PhoneNumber = orderHeaderViewModel.PhoneNumber,
                StreetAddress = orderHeaderViewModel.StreetAddress,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                OrderDate = DateTime.Now,
            };
            _unitOfWork.OrderHeaders.Add(newOrderHeader);
            _unitOfWork.Save();

            // Get All Shopping Cart for User
            var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.UserId == appUserId, nameof(Product));
            //add order details for order header
            decimal price = 0;
            foreach (var cart in shoppingCarts)
            {
                decimal unitPrice = cart.Count <= 50 ? cart.Product?.Price50 ?? 0 : cart.Product?.Price100 ?? 0;
                _unitOfWork.OrderDetails.Add(new OrderDetail
                {
                    Count = cart.Count,
                    Price = unitPrice,
                    ProductId = cart.ProductId,
                    OrderHeaderId = newOrderHeader.Id
                });
                price += unitPrice * cart.Count; 
            }
            newOrderHeader.TotalPrice = price;
            _unitOfWork.Save();
            return RedirectToAction("CreateCheckoutSession", "Payment", new { area = "Customer", orderId = newOrderHeader.Id });
        }
        [HttpPost]
        public IActionResult ConfirmOrder()
        {
            return View(nameof(ConfirmOrder));
        }
        public IActionResult Plus(int cartId)
        {
            var product = _unitOfWork.ShoppingCart.GetById(cartId);
            if (product == null)
                return NotFound("Product Not Found");

            product.Count += 1;
            _unitOfWork.ShoppingCart.Update(product);
            TempData["success"] = "+1 Added Successfully";
            TempData["success1"] = $"Updated item quantity to {product.Count}.";

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var product = _unitOfWork.ShoppingCart.GetById(cartId);
            if (product == null)
                return NotFound("Product Not Found");

            product.Count -= 1;
            if (product.Count == 0)
                return Delete(cartId);

            _unitOfWork.ShoppingCart.Update(product);
            TempData["success"] = "-1 Removed Successfully";
            TempData["success1"] = $"Updated item quantity to {product.Count}.";

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int cartId)
        {

            var product = _unitOfWork.ShoppingCart.GetById(cartId);
            if (product == null)
                return NotFound("Product Not Found");

            _unitOfWork.ShoppingCart.Remove(product);
            TempData["success"] = "Deleted Successfully";
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private decimal CalculatePriceBeasdOnCartNumber(ShoppingCart cart)
        {
            return cart.Count <= 50 ? cart.Product.Price50 : cart.Product.Price100;
        }
    }
}
