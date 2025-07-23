

namespace Ecommerce.DataAccess.Repository.Implementations
{
    public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetailsRepository
    {
        private readonly AppDbContext _context;
        public OrderDetailsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.Update(orderDetail);
        }

    }
}
