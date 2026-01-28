using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.ORM.Caching;

/// <summary>
/// Redis implementation of the cache service.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer? _redis;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisCacheService(
        IDistributedCache cache,
        ILogger<RedisCacheService> logger,
        IConnectionMultiplexer? redis = null)
    {
        _cache = cache;
        _logger = logger;
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);

            if (cached == null)
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default;
            }

            _logger.LogDebug("Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cached, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading from cache for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            var json = JsonSerializer.Serialize(value, JsonOptions);
            await _cache.SetStringAsync(key, json, options, cancellationToken);

            _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error writing to cache for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Removed cache key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error removing from cache for key: {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_redis == null)
            {
                _logger.LogWarning("IConnectionMultiplexer not available, cannot remove by prefix");
                return;
            }

            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"*{prefix}*").ToArray();

            if (keys.Length > 0)
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(keys);
                _logger.LogDebug("Removed {Count} cache keys with prefix: {Prefix}", keys.Length, prefix);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error removing cache keys by prefix: {Prefix}", prefix);
        }
    }
}
