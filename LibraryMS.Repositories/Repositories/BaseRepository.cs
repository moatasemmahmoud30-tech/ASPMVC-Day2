using EF_day3.Context;
using LibraryMS.Repositories.Interfaces;
using LibraryMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryMS.Repositories.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly LibraryContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(LibraryContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking(); 
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}