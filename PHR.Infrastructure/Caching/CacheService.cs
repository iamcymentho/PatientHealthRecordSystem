using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using PHR.Application.Abstractions.Caching;
namespace PHR.Infrastructure.Caching;
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> _cacheKeys;
    private readonly object _lock = new();
    private readonly JsonSerializerOptions _jsonOptions;
    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
        _cacheKeys = new HashSet<string>();
        _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }
    public Task<T?> GetAsync<T>(string key) where T : class
    {
        var cachedValue = _cache.Get(key);
        if (cachedValue is string json)
        {
            try
            {
                return Task.FromResult(JsonSerializer.Deserialize<T>(json, _jsonOptions));
            }
            catch
            {
                return Task.FromResult<T?>(null);
            }
        }
        return Task.FromResult(cachedValue as T);
    }
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30),
            Priority = CacheItemPriority.Normal
        };
        options.RegisterPostEvictionCallback((k, v, reason, state) =>
        {
            lock (_lock)
            {
                _cacheKeys.Remove(k.ToString()!);
            }
        });
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        _cache.Set(key, json, options);
        lock (_lock)
        {
            _cacheKeys.Add(key);
        }
        return Task.CompletedTask;
    }
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        lock (_lock)
        {
            _cacheKeys.Remove(key);
        }
        return Task.CompletedTask;
    }
    public Task RemoveByPatternAsync(string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var keysToRemove = new List<string>();
        lock (_lock)
        {
            foreach (var key in _cacheKeys)
            {
                if (regex.IsMatch(key))
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _cacheKeys.Remove(key);
            }
        }
        return Task.CompletedTask;
    }
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> getItem, TimeSpan? expiration = null) where T : class
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }
        var value = await getItem();
        if (value != null)
        {
            await SetAsync(key, value, expiration);
        }
        return value;
    }
}