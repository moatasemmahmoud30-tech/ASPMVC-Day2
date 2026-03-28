using EF_day3.Context;   
using EF_day3.Entities;
using LibraryMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryMS.Repositories.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private LibraryContext _appContext => (LibraryContext)_context;

        public BookRepository(LibraryContext context) : base(context)
        {
        }

        public IQueryable<Book> GetFilteredAndPagedBooks(
            string searchTerm,
            int? categoryId,
            decimal? maxPrice,
            int pageNumber,
            int pageSize)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b =>
                    (b.Title != null && b.Title.Contains(searchTerm)) ||
                    (b.Author != null && b.Author.Name != null && b.Author.Name.Contains(searchTerm))
                );
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value);
            }

            query = query.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            return query;
        }

        public Book GetBookWithDetails(int id)
        {
            return _dbSet
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Attachments)
                .FirstOrDefault(b => b.Id == id);
        }
    }
}