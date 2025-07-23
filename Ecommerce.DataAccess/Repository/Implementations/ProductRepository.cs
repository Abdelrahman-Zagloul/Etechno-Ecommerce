
namespace Ecommerce.DataAccess.Repository.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            var exist = _context.Products.Find(product.Id);
            if (exist != null)
            {
                exist.ISBN = product.ISBN;
                exist.Name = product.Name;
                exist.Description = product.Description;
                exist.ListPrice= product.ListPrice;
                exist.Price50 = product.Price50;
                exist.Price100 = product.Price100;
                exist.Author = product.Author;
                exist.CategoryId = product.CategoryId;
                exist.ImageUrl = product.ImageUrl;
            }

        }
    }
}
