using ASPMVC_Day1.Models;
using ASPMVC_Day1.ViewModels;
using EF_day3.Entities;
using LibraryMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Hosting;

namespace ASPMVC_Day1.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IBookRepository _bookRepository;

        private readonly IBaseRepository<Author> _authorRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IBaseRepository<BookAttachment> _attachmentRepository;

        public BookController(
            IBookRepository bookRepository,
            IBaseRepository<Author> authorRepository,
            IBaseRepository<Category> categoryRepository,
            IBaseRepository<BookAttachment> attachmentRepository,
            IWebHostEnvironment env)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _attachmentRepository = attachmentRepository;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index(string searchString, int? categoryId, decimal? maxPrice, int pageNumber = 1)
        {
            int pageSize = 6;

            var booksQuery = _bookRepository.GetFilteredAndPagedBooks(
                searchString, categoryId, maxPrice, pageNumber, pageSize);

            var booksList = booksQuery.Select(b => new BookCardViewModel
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                Price = b.Price,
                CoverImageUrl = b.Attachments.FirstOrDefault() != null ? b.Attachments.FirstOrDefault().FilePath : null
            }).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;
            ViewBag.MaxPrice = maxPrice;

            ViewBag.Categories = _categoryRepository.GetAll().ToList();

            return View(booksList);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var book = _bookRepository.GetBookWithDetails(id);

            if (book == null) return NotFound();

            var model = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                CategoryId = book.CategoryId,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishYear = book.PublishYear,
                Price = book.Price,
                AuthorName = book.Author?.Name ?? "Unknown Author",
                CategoryName = book.Category?.Name ?? "Uncategorized",
                AttachmentUrls = book.Attachments?.Select(a => a.FilePath).ToList() ?? new List<string>()
            };

            return View(model);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Authors = _authorRepository.GetAll().ToList();
            ViewBag.Categories = _categoryRepository.GetAll().ToList();
            return View();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult Add(BookCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newBook = new EF_day3.Entities.Book
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
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

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

                _bookRepository.Add(newBook);
                return RedirectToAction("Index");
            }

            ViewBag.Authors = _authorRepository.GetAll().ToList();
            ViewBag.Categories = _categoryRepository.GetAll().ToList();
            return View(model);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _bookRepository.GetBookWithDetails(id);
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
                ExistingAttachments = book.Attachments?.ToList() ?? new List<BookAttachment>()
            };

            ViewBag.Authors = _authorRepository.GetAll().ToList();
            ViewBag.Categories = _categoryRepository.GetAll().ToList();
            return View(model);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult Edit(BookEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingBook = _bookRepository.GetBookWithDetails(model.Id);
                if (existingBook == null) return NotFound();

                existingBook.Title = model.Title;
                existingBook.AuthorId = model.AuthorId;
                existingBook.ISBN = model.ISBN;
                existingBook.CategoryId = model.CategoryId;
                existingBook.PublishYear = model.PublishYear;
                existingBook.Price = model.Price;
                existingBook.Version = model.Version; 

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

                _bookRepository.Update(existingBook);
                return RedirectToAction("Index");
            }

            ViewBag.Authors = _authorRepository.GetAll().ToList();
            ViewBag.Categories = _categoryRepository.GetAll().ToList();
            return View(model);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public IActionResult DeleteAttachment(int attachmentId, int bookId)
        {
            var attachment = _attachmentRepository.GetById(attachmentId);

            if (attachment != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                string fileName = attachment.FilePath.Replace("/uploads/", "");
                string fullFilePath = Path.Combine(uploadsFolder, fileName);

                if (System.IO.File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                }

                _attachmentRepository.Delete(attachment);
            }

            return RedirectToAction("Edit", new { id = bookId });
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var book = _bookRepository.GetBookWithDetails(id);
            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _bookRepository.GetBookWithDetails(id);

            if (book != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                if (book.Attachments != null)
                {
                    foreach (var attachment in book.Attachments)
                    {
                        string fileName = attachment.FilePath.Replace("/uploads/", "");
                        string fullFilePath = Path.Combine(uploadsFolder, fileName);

                        if (System.IO.File.Exists(fullFilePath))
                        {
                            System.IO.File.Delete(fullFilePath);
                        }
                    }
                }

                _bookRepository.Delete(book);
            }
            return RedirectToAction("Index");
        }
    }
}