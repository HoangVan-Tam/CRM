using System.Linq.Expressions;

namespace DAL.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        T GetById(object id);
        void Insert(T obj);
        Task InsertAsync(T obj);
        void Update(T obj);
        void Delete(object id);
        Task<T> FindAsync(Expression<Func<T, bool>> filterExpression);
        Task<T> FindAsyncWithIncludeAsync(Expression<Func<T, bool>> filterExpression, params Expression<Func<T, Object>>[] includes);
        Task<T> FindAsync(Expression<Func<T, bool>> filterExpression, params Expression<Func<T, Object>>[] includes);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> filterExpression);
        Task<List<T>> FindAllWithIncludeAsync(Expression<Func<T, bool>> filterExpression, params Expression<Func<T, Object>>[] includes);
    }
}
