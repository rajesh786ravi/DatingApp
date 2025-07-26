using Microsoft.AspNetCore.Mvc;
using AzureQueueDemo.Services;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly AzureQueueService _queue;

    public QueueController(AzureQueueService queue)
    {
        _queue = queue;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] string message)
    {
        await _queue.SendMessageAsync(message);
        return Ok("Sent");
    }

    [HttpGet("receive")]
    public async Task<IActionResult> Receive()
    {
        var msg = await _queue.ReceiveMessageAsync();
        return Ok(msg ?? "No messages in queue");
    }
}
