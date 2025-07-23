
namespace Ecommerce.DataAccess.Repository.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product category);
    }
}
