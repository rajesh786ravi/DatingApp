using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/[controller]")]
public class CacheController : ControllerBase
{
    private readonly IMemoryCache _cache;
    public CacheController(IMemoryCache cache)
    {
        _cache = cache;
    }

    [HttpGet("{id}")]
    public IActionResult GetCache(int id)
    {
        string cacheKey = $"cache_{id}";
        if (!_cache.TryGetValue(cacheKey, out string? cache))
        {
            // simulate DB or external API call
            cache = $"Product details for ID {id} - Fetched from DB";

            // store in cache for 5 minutes
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, cache, cacheOptions);
        }

        return Ok(cache);
    }

}