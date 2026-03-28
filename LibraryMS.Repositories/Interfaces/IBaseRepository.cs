using System.Linq;
using System.Linq.Expressions;

namespace LibraryMS.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        T GetById(int id);
        IQueryable<T> GetAll();

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}