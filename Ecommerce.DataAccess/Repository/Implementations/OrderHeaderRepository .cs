

namespace Ecommerce.DataAccess.Repository.Implementations
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext _context;
        public OrderHeaderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.Update(orderHeader);
        }
        public void UpdateStatus(int orderHeaderId, string orderStatus, string? PaymentStatus = null)
        {
            var orderFromDb = _context.OrderHeaders.Find(orderHeaderId);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(PaymentStatus))
                {
                    orderFromDb.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdatePaymentIntendId(int orderHeaderId, string sessionId, string paymentIntendId)
        {
            var orderFromDb = _context.OrderHeaders.Find(orderHeaderId);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                    orderFromDb.SessionId = sessionId;
                if (!string.IsNullOrEmpty(paymentIntendId))
                {
                    orderFromDb.PaymentIntentId = paymentIntendId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
            }
        }

    }
}
