using System.Diagnostics;

public class RequestTimingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Before the next middleware / request handling
        Console.WriteLine($"[Middleware] Request starting: {context.Request.Method} {context.Request.Path}");

        await _next(context); // Pass control to the next middleware

        // After response is generated
        stopwatch.Stop();
        Console.WriteLine($"[Middleware] Request finished in {stopwatch.ElapsedMilliseconds}ms");
    }
}