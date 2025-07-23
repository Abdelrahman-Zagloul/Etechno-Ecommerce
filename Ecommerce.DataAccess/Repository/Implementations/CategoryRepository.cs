

namespace Ecommerce.DataAccess.Repository.Implementations
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            _context.Update(category);
        }

    }
}
