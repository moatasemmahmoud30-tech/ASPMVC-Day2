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

        public IActionResult Details(string id)
        {
            var book = _context.Books.FirstOrDefault(b => b.ISBN == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Edit(string id)
        {
            return View();
        }

        public IActionResult Delete(string id)
        {
            return View();
        }
    }
}