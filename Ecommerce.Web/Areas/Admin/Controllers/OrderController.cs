using Stripe;
namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaders.Get(x => x.Id == id);
            var orderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderHeaderId == orderHeader.Id, "Product").ToList();
            var user = await _userManager.FindByIdAsync(orderHeader.ApplicationUserId);
            string? email = user?.Email;
            var orderVM = new OrderViewModel
            {
                Header = orderHeader,
                Details = orderDetails,
                Email = email,
                TotalPrice = orderDetails.Sum((x => x.Price * x.Count))
            };
            return View(nameof(Details), orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult UpdateOrderDetails(int orderHeaderId, OrderViewModel orderViewModel)
        {
            var orderFromDb = _unitOfWork.OrderHeaders.Get(x => x.Id == orderHeaderId);
            if (orderFromDb == null)
                TempData["error"] = "Order Not Found";

            orderFromDb.Name = orderViewModel.Header.Name;
            orderFromDb.PhoneNumber = orderViewModel.Header.PhoneNumber;
            orderFromDb.StreetAddress = orderViewModel.Header.StreetAddress;
            orderFromDb.City = orderViewModel.Header.City;

            _unitOfWork.OrderHeaders.Update(orderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Updated Successfully";
            return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult StartProcessing(int orderHeaderId)
        {
            _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderId, OrderStatus.Processing);
            _unitOfWork.Save();
            TempData["success"] = "Order Processing Now..... ";
            return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult ShipOrder(int orderHeaderId, OrderViewModel orderViewModel)
        {
            var orderFromDb = _unitOfWork.OrderHeaders.Get(x => x.Id == orderHeaderId);
            if (orderFromDb == null)
            {
                TempData["error"] = "Order Not Found";
                return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Carrier Can't Be Null";
                return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
            }

            orderFromDb.Carrier = orderViewModel.Header.Carrier;
            orderFromDb.TrackingNumber = Guid.NewGuid().ToString();
            orderFromDb.ShippingDate = DateTime.Now;
            orderFromDb.OrderStatus = OrderStatus.Shipped;
            _unitOfWork.OrderHeaders.Update(orderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully";
            if (!new EmailAddressAttribute().IsValid(orderViewModel.Email))
            {
                TempData["error"] = "Email address is invalid.";
                return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
            }
            _ = Task.Run(async () =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(orderViewModel.Email))
                    {
                        string returnUrl = $"{SD.returnUrl}{orderHeaderId}";
                        string loginUrl = $"{SD.Domain}Authentication/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";

                        string emailBody = $@"
                 <h2>Your Order Is Shipped</h2>
                 <p>Order ID: {orderHeaderId}</p>
                 <p>Carrier Name: {orderFromDb.Carrier ?? "Unknown"}</p>
                 <p>Tracking Number: {orderFromDb.TrackingNumber ?? "Not Available"}</p>
                 <a href='{loginUrl}'>View Your Order</a>
             ";

                        await _emailSender.SendEmailAsync(
                            orderViewModel.Email,
                            "Your Order Has Been Shipped",
                            emailBody
                        );
                    }
                }
                catch (Exception ex) { }
            });
            return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDelivered(int orderHeaderId)
        {
            var orderFromDb = _unitOfWork.OrderHeaders.Get(x => x.Id == orderHeaderId);
            if (orderFromDb == null)
            {
                TempData["error"] = "Order Not Found";
                return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
            }
            var user = await _userManager.FindByIdAsync(orderFromDb.ApplicationUserId);

            _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderId, OrderStatus.Delivered);
            _unitOfWork.Save();
            TempData["success"] = "Order Delivered Successfully";

            if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                TempData["error"] = "Email address is invalid.";
                return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
            }
            _ = Task.Run(async () =>
            {
                try
                {

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Your Order Has Been Delivered",
                  @"<h3>Order Delivered Successfully</h3>
                         <p>Thank you for shopping with us. We hope to see you again soon!</p>"

                    );
                }
                catch (Exception ex) { }
            });
            return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderHeaderId)
        {
            var orderFromDb = _unitOfWork.OrderHeaders.Get(x => x.Id == orderHeaderId);
            if (orderFromDb == null)
            {
                TempData["error"] = "Order Not Found";
                return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
            }
            var user = await _userManager.FindByIdAsync(orderFromDb.ApplicationUserId);
            if (user == null || !new EmailAddressAttribute().IsValid(user.Email))
            {
                TempData["error"] = "Email address is invalid.";
                return RedirectToAction(nameof(Details), new { area = "Admin", id = orderHeaderId });
            }
            if (orderFromDb.PaymentStatus == PaymentStatus.Approved)
            {
                var option = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntentId,
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderId, OrderStatus.Cancelled, PaymentStatus.Refunded);
                TempData["success"] = "Order Cancelled Successfully";
                _unitOfWork.Save();
                SendCancelEmailAsync(user.Email!, true);
                return View(nameof(CancelOrder), true);
            }
            else
            {
                _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderId, OrderStatus.Cancelled, PaymentStatus.Rejected);
                TempData["success"] = "Order Cancelled Without Refund";
                _unitOfWork.Save();
                SendCancelEmailAsync(user.Email!, false);
                return View(nameof(CancelOrder), false);
            }
        }


        #region API Call
        [HttpGet]
        public IActionResult GetAll(string? orderStatus, string? paymentStatus)
        {

            IQueryable<OrderHeader> orderListQuery;

            if (User.IsInRole(SD.AdminRole))
                orderListQuery = _unitOfWork.OrderHeaders.GetAll(null, "OrderDetails");
            else
            {
                var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                orderListQuery = _unitOfWork.OrderHeaders.GetAll(x => x.ApplicationUserId == appUserId, "OrderDetails");
            }

            // for orderStatus 
            if (!string.IsNullOrEmpty(orderStatus))
                orderListQuery = orderListQuery.Where(o => o.OrderStatus == orderStatus);

            // for paymentStatus 
            if (!string.IsNullOrEmpty(paymentStatus))
                orderListQuery = orderListQuery.Where(o => o.PaymentStatus == paymentStatus);


            var orderList = orderListQuery.Select(x => new
            {
                x.Id,
                x.Name,
                x.PhoneNumber,
                x.PaymentStatus,
                x.OrderStatus,
                Total = x.OrderDetails.Sum(od => od.Price * od.Count)
            });

            return Json(new { data = orderList });
        }

        #endregion
        private void SendCancelEmailAsync(string email, bool isRefunded)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    string subject = "Your Order Has Been Canceled";
                    string message = isRefunded
                        ? @"<h3>Order Canceled</h3><p>Your order was cancelled and refunded successfully. We hope to see you again soon!</p>"
                        : @"<h3>Order Canceled</h3><p>Your order was cancelled. No payment was made. We hope to see you again soon!</p>";

                    await _emailSender.SendEmailAsync(email, subject, message);
                }
                catch { }
            });
        }
    }
}
