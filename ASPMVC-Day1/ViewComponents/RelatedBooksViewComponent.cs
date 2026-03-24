using ASPMVC_Day1.ViewModels;
using EF_day3.Context;
using EF_day3.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ASPMVC_Day1.ViewComponents
{
    public class RelatedBooksViewComponent : ViewComponent
    {
        private readonly LibraryContext _context;

        public RelatedBooksViewComponent(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentBookId, int categoryId, int authorId)
        {
            var relatedBooks = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Attachments)
                .Where(b => b.Id != currentBookId && (b.CategoryId == categoryId || b.AuthorId == authorId))
                .OrderByDescending(b => b.PublishYear) 
                .Take(4) 
                .Select(b => new BookCardViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                    Price = b.Price,
                    CoverImageUrl = b.Attachments.OrderBy(a => a.Id).FirstOrDefault() != null
                                    ? b.Attachments.OrderBy(a => a.Id).FirstOrDefault().FilePath
                                    : null
                })
                .ToListAsync();

            return View(relatedBooks);
        }
    }
}
