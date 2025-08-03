using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly CosmosDbService _cosmosDbService;

    public ItemsController(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(MyItem item)
    {
        await _cosmosDbService.AddItemAsync(item);
        return Ok("Item created successfully");
    }

    [HttpGet("{id}/{partitionKey}")]
    public async Task<IActionResult> Get(string id, string partitionKey)
    {
        var item = await _cosmosDbService.GetItemAsync<MyItem>(id, partitionKey);
        if (item == null) return NotFound();
        return Ok(item);
    }
}
