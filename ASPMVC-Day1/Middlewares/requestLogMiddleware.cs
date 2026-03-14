using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class RequestLogMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"Incoming Request: {context.Request.Path}");

        await _next(context);
    }
}