using Newtonsoft.Json;

public class MyItem
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("partitionKey")]
    public string? PartitionKey { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }
}
