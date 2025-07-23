

namespace Ecommerce.DataAccess.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includes = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(List<T> entities);
        void Clear();
        T? GetById(int id);
        T? Get(Expression<Func<T, bool>> filter, string? includes = null);
    }
}
