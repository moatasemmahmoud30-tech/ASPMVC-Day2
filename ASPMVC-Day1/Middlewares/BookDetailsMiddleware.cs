using EF_day3.Context;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using EF_day3.Context;
using EF_day3.Entities;

namespace ASPMVC_Day1.Middlewares
{
    public class BookDetailsMiddleware
    {
        private readonly RequestDelegate _next;

        public BookDetailsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, LibraryContext dbContext)
        {
            if (context.Request.Path.StartsWithSegments("/bookInfo", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentType = "text/plain; charset=utf-8";

                if (context.Request.Query.TryGetValue("isbn", out var isbnValues))
                {
                    string isbn = isbnValues.ToString();
                    var book = dbContext.Books.FirstOrDefault(b => b.ISBN == isbn);
                    if (book != null)
                    {
                        await context.Response.WriteAsync($"=== Book Details ===\n");
                        await context.Response.WriteAsync($"Title: {book.Title}\n");
                        await context.Response.WriteAsync($"Price: {book.Price}$\n");
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync($"No book found matching ISBN: {isbn}");
                    }
                }
                else
                {
                    context.Response.StatusCode = 400; 
                    await context.Response.WriteAsync("Error: Please provide an ISBN in the URL (e.g., /bookInfo?isbn=12345)");
                }

                return;
            }

            await _next(context);
        }
    }
}