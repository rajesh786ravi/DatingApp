using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

public class CosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(CosmosClient client, string databaseName, string containerName)
    {
        _container = client.GetContainer(databaseName, containerName);
    }

    public async Task AddItemAsync<T>(T item) where T : class
    {
        await _container.CreateItemAsync(item);
    }

    public async Task<T> GetItemAsync<T>(string id, string partitionKey) where T : class
    {
        var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        return response.Resource;
    }
}
