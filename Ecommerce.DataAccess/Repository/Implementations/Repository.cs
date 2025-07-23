
namespace Ecommerce.DataAccess.Repository.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Clear()
        {
            _dbSet.RemoveRange(_dbSet);
        }

        public T? Get(Expression<Func<T, bool>> filter, string? includes = null)
        {

            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var includeProp in includes.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            query = query.Where(filter);
            return query.FirstOrDefault();

        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includes = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var includeProp in includes.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query;
        }

        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(List<T> entities)
        {
            _context.RemoveRange(entities);
        }
    }

}

