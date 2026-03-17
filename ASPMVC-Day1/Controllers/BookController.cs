using ASPMVC_Day1.ViewModels;
using EF_day3.Context;
using EF_day3.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ASPMVC_Day1.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryContext _context;
        private readonly IWebHostEnvironment _env; 

        public BookController(LibraryContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = new List<string> { "All columns", "Fiction", "Dystopian", "Science" };

            var books = _context.Books.ToList();

            return View(books);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Attachments)
                .FirstOrDefault(b => b.Id == id);

            if (book == null) return NotFound();

            var model = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishYear = book.PublishYear,
                Price = book.Price,
                AuthorName = book.Author?.Name ?? "Unknown Author",
                CategoryName = book.Category?.Name ?? "Uncategorized",
                AttachmentUrls = book.Attachments.Select(a => a.FilePath).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Add(BookCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newBook = new Book
                {
                    Title = model.Title,
                    AuthorId = model.AuthorId,
                    CategoryId = model.CategoryId,
                    ISBN = model.ISBN,
                    PublishYear = model.PublishYear,
                    Price = model.Price,
                    Attachments = new List<BookAttachment>()
                };

                if (model.Files != null && model.Files.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in model.Files)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        newBook.Attachments.Add(new BookAttachment
                        {
                            FileName = file.FileName,
                            FilePath = "/uploads/" + uniqueFileName 
                        });
                    }
                }

                _context.Books.Add(newBook);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b => b.Attachments).FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            var model = new BookEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                CategoryId = book.CategoryId,
                PublishYear = book.PublishYear,
                Price = book.Price,
                Version = book.Version,
                ExistingAttachments = book.Attachments.ToList()
            };

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(BookEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingBook = _context.Books.Include(b => b.Attachments).FirstOrDefault(b => b.Id == model.Id);

                if (existingBook == null) return NotFound();

                existingBook.Title = model.Title;
                existingBook.AuthorId = model.AuthorId;
                existingBook.ISBN = model.ISBN;
                existingBook.CategoryId = model.CategoryId;
                existingBook.PublishYear = model.PublishYear;
                existingBook.Price = model.Price;

                _context.Entry(existingBook).OriginalValues["Version"] = model.Version;

                if (model.NewFiles != null && model.NewFiles.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                    foreach (var file in model.NewFiles)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        existingBook.Attachments.Add(new BookAttachment
                        {
                            FileName = file.FileName,
                            FilePath = "/uploads/" + uniqueFileName
                        });
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteAttachment(int attachmentId, int bookId)
        {
            var attachment = _context.Set<BookAttachment>().Find(attachmentId);

            if (attachment != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                string fileName = attachment.FilePath.Replace("/uploads/", "");
                string fullFilePath = Path.Combine(uploadsFolder, fileName);

                if (System.IO.File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                }

                _context.Set<BookAttachment>().Remove(attachment);
                _context.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = bookId });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _context.Books.Include(b => b.Attachments).FirstOrDefault(b => b.Id == id);

            if (book != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                foreach (var attachment in book.Attachments)
                {
                    string fileName = attachment.FilePath.Replace("/uploads/", "");
                    string fullFilePath = Path.Combine(uploadsFolder, fileName);

                    if (System.IO.File.Exists(fullFilePath))
                    {
                        System.IO.File.Delete(fullFilePath); 
                    }
                }
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}