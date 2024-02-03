using System.Linq.Expressions;

namespace AuthDemo.Domain.Repositories.Interfaces
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        T Get(TKey id);
        IQueryable<T> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
