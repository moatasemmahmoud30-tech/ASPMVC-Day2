using EF_day3.Entities;
using EF_day3.Entities;
using LibraryMS.Repositories.Interfaces;
using System.Linq;

namespace LibraryMS.Repositories.Interfaces
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        IQueryable<Book> GetFilteredAndPagedBooks(
            string searchTerm,
            int? categoryId,
            decimal? maxPrice,
            int pageNumber,
            int pageSize);
        Book GetBookWithDetails(int id);
    }
}