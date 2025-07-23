namespace Ecommerce.DataAccess.Repository.Interfaces
{
    public interface IOrderDetailsRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
