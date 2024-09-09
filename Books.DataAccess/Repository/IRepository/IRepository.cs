using System.Linq.Expressions;

namespace Books.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T - Category
        IEnumerable<T> GetAll(string? includeProperties = null);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, string? includeProperties = null);

        T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);

        void Add(T item);

        void Remove(T item);

        void RemoveRange(IEnumerable<T> items);
    }
}
