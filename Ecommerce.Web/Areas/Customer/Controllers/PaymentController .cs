using Stripe.Checkout;
namespace Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public PaymentController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult CreateCheckoutSession(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeaders.Get(x => x.Id == orderId);
            var orderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderHeaderId == orderId, nameof(Product));
            var domain = SD.Domain;
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{SD.SuccessUrl}orderId={orderId}",
                CancelUrl = SD.CancelUrl,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var item in orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            // session id change and payment 
            _unitOfWork.OrderHeaders.UpdatePaymentIntendId(orderId, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            return Redirect(session.Url);
        }
        public IActionResult Success(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeaders.GetById(orderId);
            if (orderHeader == null)
                return NotFound();

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() != "paid")
            {
                TempData["error"] = " Payment Failed";
                return View("Error");
            }
            try
            {
                _unitOfWork.OrderHeaders.UpdatePaymentIntendId(orderId, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeaders.UpdateStatus(orderId, OrderStatus.Approved, PaymentStatus.Approved);
                TempData["Success"] = "Added Order successfully!";
                var shoppingCard = _unitOfWork.ShoppingCart.GetAll(x => x.UserId == orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCart.RemoveRange(shoppingCard);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.CardNumber, 0);
                TempData["Success1"] = "Payment Completed successfully!";

                //send email to user and admin
                _ = Task.Run(async () =>
                {
                    try
                    {
                        string returnUrl = $"{SD.returnUrl}{orderId}";
                        string loginUrl = $"{SD.Domain}Authentication/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";

                        string emailBody = $@"
                            <h2>New Order Received</h2>
                            <p>Order ID: {orderId}</p>
                            <p>User: {orderHeader.Name ?? "Unknown User"}</p>
                            <a href='{loginUrl}'>Manage Order</a>
                        ";


                        await _emailSender.SendEmailAsync(
                            SD.AdminEmail,
                            "New Order Placed",
                            emailBody
                        );
                    }
                    catch (Exception ex) { }
                });
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(
                            User.FindFirstValue(ClaimTypes.Email) ?? "",
                            "Payment Successfully",
                            "<h2>Thank you for your order \n We've received your payment successfully! and your order is being processed.</p>"
                        );
                    }
                    catch (Exception ex) { }
                });
                return View("Success", orderHeader.Id);
            }
            catch
            {
                TempData["error"] = "Something went wrong while processing the payment.";
                return View("Error");
            }
        }
        public IActionResult Cancel()
        {
            TempData["error"] = "Payment Cancelled";
            return RedirectToAction("Summary", "Cart", new { area = "Customer" });
        }
        public IActionResult Error()
        {
            return View();
        }
    }

}

