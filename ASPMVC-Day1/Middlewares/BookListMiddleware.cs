using EF_day3.Context;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using EF_day3.Entities;

namespace ASPMVC_Day1.Middlewares
{
    public class BookListMiddleware
    {
        private readonly RequestDelegate _next;

        public BookListMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, LibraryContext dbContext)
        {
            if (context.Request.Path.StartsWithSegments("/book", StringComparison.OrdinalIgnoreCase))
            {

                var books = dbContext.Books.ToList();

                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("=== Library Book List ===\n\n");

                foreach (var book in books)
                {
                    await context.Response.WriteAsync($"Title: {book.Title} | Price: {book.Price}$\n");
                }
                return;
            }

            await _next(context);
        }
    }
}