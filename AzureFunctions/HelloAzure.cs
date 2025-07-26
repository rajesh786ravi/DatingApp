using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Function
{
    public class HelloAzure(ILogger<HelloAzure> logger)
    {
        private readonly ILogger<HelloAzure> _logger = logger;

        [Function("HelloAzure")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string? name = req.Query["name"];
            if (string.IsNullOrEmpty(name)) name = "Boss";
            return new OkObjectResult($"Hello {name}, Welcome to Azure Functions!");
        }
    }
}