using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;

namespace AzureQueueDemo.Services;

public class AzureQueueService
{
    private readonly QueueClient _queueClient;

    public AzureQueueService(IConfiguration config)
    {
        var conn = config["AzureQueue:ConnectionString"];
        var name = config["AzureQueue:QueueName"];

        _queueClient = new QueueClient(conn, name);
        _queueClient.CreateIfNotExists();
    }

    public async Task SendMessageAsync(string message)
    {
        await _queueClient.SendMessageAsync(message);
    }

    public async Task<string?> ReceiveMessageAsync()
    {
        var msg = await _queueClient.ReceiveMessageAsync();
        if (msg.Value != null)
        {
            await _queueClient.DeleteMessageAsync(msg.Value.MessageId, msg.Value.PopReceipt);
            return msg.Value.MessageText;
        }
        return null;
    }
}
