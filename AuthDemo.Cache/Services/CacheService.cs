using AuthDemo.Cache.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using static AuthDemo.Cache.Constants.CacheKeys;

namespace AuthDemo.Cache.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;
        public CacheService(IDatabase cacheDb)
        {
            _cacheDb = cacheDb;
        }


        public async Task<T?> GetDataAsync<T>(string key)
        {
            var value = await _cacheDb.StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;

        }

        public async Task<bool> SetDataAsync<T>(string key, T value)
        {
            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value));
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTime expiration)
        {
            var expirationTime = expiration - DateTime.UtcNow;
            return await _cacheDb.StringSetAsync(key, JsonSerializer.Serialize(value), expirationTime);
        }


        public async Task<bool> RemoveDataAsync(string key)
        {
            bool value = await _cacheDb.KeyExistsAsync(key);
            if (value)
            {
                await _cacheDb.KeyDeleteAsync(key);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveResourcePerObjectIdAsync(CacheResources resource, string resourceId, string objectId)
        {
            var key = $"{App}:{resource.GetEnumName()}:{objectId}:{resourceId}";
            bool value = await _cacheDb.KeyExistsAsync(key);
            if (value)
            {
                await _cacheDb.KeyDeleteAsync(key);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveAllResourcesPerObjectIdAsync(CacheResources resource, string objectId)
        {
            var pattern = $"{App}:{resource.GetEnumName()}:{objectId}:*";
            var keys = GetRedisKeysForPattern(pattern);
            if (keys == null)
            {
                return true;
            }
            var deletionTasks = keys.Select(async key => await _cacheDb.KeyDeleteAsync(key)).ToArray();
            bool[] deletionResults = await Task.WhenAll(deletionTasks);
            return deletionResults.All(result => result);

        }

        public async Task<bool> SetResourceDataPerObjectIdAsync<T>(CacheResources resource, string resourceId, T resourceValue, string objectId, DateTime expiration)
        {
            var newCacheKey = $"{App}:{resource.GetEnumName()}:{objectId}:{resourceId}";
            var expirationTime = expiration - DateTime.UtcNow;
            return await _cacheDb.StringSetAsync(newCacheKey, JsonSerializer.Serialize(resourceValue), expirationTime);
        }

        public async Task<T> GetResourcePerObjectIdAsync<T>(CacheResources resource, string resourceId, string objectId)
        {
            var key = $"{App}:{resource.GetEnumName()}:{objectId}:{resourceId}";
            var value = await _cacheDb.StringGetAsync(key);

            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public async Task<IEnumerable<T>> GetAllResourcesPerObjectIdAsync<T>(CacheResources resource, string objectId)
        {
            List<T> result = [];
            var pattern = $"{App}:{resource.GetEnumName()}:{objectId}:*";

            var keys = GetRedisKeysForPattern(pattern);
            if (keys == null)
            {
                return result;
            }

            foreach (var key in keys)
            {
                var value = await _cacheDb.StringGetAsync(key);
                if (value.HasValue)
                {
                    result.Add(JsonSerializer.Deserialize<T>(value));
                }
            }
            return result;
        }

        private RedisKey[]? GetRedisKeysForPattern(string pattern)
        {
            // it si also possibel to limit/pagination with
            // _cacheDb.Execute("SCAN", "0", "MATCH", pattern, "COUNT", "1000");
            var keys = _cacheDb.Execute("SCAN", "0", "MATCH", pattern);
            if (keys.IsNull)
            {
                return null;
            }
            var keyArray = (RedisResult[])keys;
            return (RedisKey[]?)keyArray[1];
        }
    }

    public static class EnumExtensions
    {
        public static string GetEnumName<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            return Enum.GetName(typeof(TEnum), value);
        }
    }
}
