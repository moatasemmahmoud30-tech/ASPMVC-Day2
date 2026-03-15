using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using EF_day3.Context;
using EF_day3.Entities;

namespace ASPMVC_Day1.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = new List<string> { "All columns", "Fiction", "Dystopian", "Science" };

            var books = _context.Books.ToList();

            return View(books);
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Add(Book newBook)
        {
            ModelState.Remove("Author");
            ModelState.Remove("Category");
            ModelState.Remove("Loans");
            ModelState.Remove("Version");

            if (newBook.Price == 0) newBook.Price = 19.99m;

            if (ModelState.IsValid)
            {
                _context.Books.Add(newBook);
                _context.SaveChanges(); 

                return Redirect("/Book/Index"); 
            }

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(newBook);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();

            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book updatedBook)
        {
            ModelState.Remove("Author");
            ModelState.Remove("Category");
            ModelState.Remove("Loans");
            ModelState.Remove("Version");

            if (ModelState.IsValid)
            {
                _context.Books.Update(updatedBook);
                _context.SaveChanges();
                return Redirect("/Book/Index"); 
            }

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(updatedBook);
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
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return Redirect("/Book/Index");
        }
    }
}