namespace Ecommerce.DataAccess.Repository.Interfaces
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int orderHeaderId, string orderStatus, string? PaymentStatus = null);
        void UpdatePaymentIntendId(int orderHeaderId, string sessionId, string paymentIntendId);
    }
}
