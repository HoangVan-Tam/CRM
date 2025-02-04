

using DAL.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace DAL.Implement
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private StandardContest2023Context _context;
        private DbSet<T> table;

        public GenericRepository(StandardContest2023Context context)
        {
            _context = context;
            table = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }
        public T GetById(object id)
        {
            return table.Find(id);
        }
        public void Insert(T obj)
        {
            table.Add(obj);
        }
        public async Task InsertAsync(T obj)
        {
            await table.AddAsync(obj);
        }
        public void Update(T obj)
        {
            table.Update(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> filterExpression)
        {
            return await table.Where(filterExpression).FirstOrDefaultAsync();
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> filterExpression, params Expression<Func<T, Object>>[] includes)
        {
            IQueryable<T> query = null;
            foreach (var include in includes)
            {
                query = includes.Aggregate(query,
                  (current, include) => current.Include(include));
            }
            query = query.Where(filterExpression);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> filterExpression)
        {
            return await table.Where(filterExpression).ToListAsync();
        }
        public async Task<List<T>> FindAllWithIncludeAsync(Expression<Func<T, bool>> filterExpression, params Expression<Func<T, Object>>[] includes)
        {
            IQueryable<T> query = null;
            foreach (var include in includes)
            {
                query = table.Include(include);
            }
            query = query.Where(filterExpression);
            return await query.ToListAsync();
        }
    }
}
