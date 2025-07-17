using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public class HelloAzure
{
    private readonly ILogger<HelloAzure> _logger;

    public HelloAzure(ILogger<HelloAzure> logger)
    {
        _logger = logger;
    }

    [Function("HelloAzure")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        string? name = req.Query["name"];
        if (string.IsNullOrEmpty(name)) name = "Boss";
        return new OkObjectResult($"Hello {name}, Welcome to Azure Functions!");
    }
}