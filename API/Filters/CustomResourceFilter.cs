using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class CustomResourceFilter : IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        // Runs BEFORE model binding
        var method = context.HttpContext.Request.Method;
        var path = context.HttpContext.Request.Path;
        Console.WriteLine($"🔍 {method} {path} - Resource Executing");

        // Example: Short-circuit request
        // context.Result = new ContentResult { Content = "Request Blocked" };
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        // Runs AFTER the rest of the pipelinevar method = context.HttpContext.Request.Method;
        var method = context.HttpContext.Request.Method;
        var path = context.HttpContext.Request.Path;
        Console.WriteLine($"🔍 {method} {path} - Resource Executed");
    }
}
