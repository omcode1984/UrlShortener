using Microsoft.Extensions.Caching.Memory;
using System;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public virtual void Set(string key, string value)
    {
        _memoryCache.Set(key, value, TimeSpan.FromDays(30)); // Set cache expiration to 30 days
    }

    public virtual string Get(string key)
    {
        _memoryCache.TryGetValue(key, out string value);
        return value;
    }

    public virtual bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }
}
