using AuthDemo.Cache.Interfaces;
using StackExchange.Redis;
using System.Text;
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

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expiration)
        {
            var expirationTime = expiration - DateTimeOffset.UtcNow;
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

        
        public async Task<bool> RemoveAllResourcesPerIdAsync(CacheResources resource, string objectId)
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

        public async Task<bool> SetResourceDataPerKeyValuePairsAsync<T>(List<KeyValuePair<CacheResources, string>> cacheKeyPairs, T cacheValue, DateTimeOffset expiration)
        {
            var newCacheKey = GenerateCacheKey(cacheKeyPairs);
            var expirationTime = expiration - DateTimeOffset.UtcNow;
            return await _cacheDb.StringSetAsync(newCacheKey, JsonSerializer.Serialize(cacheValue), expirationTime);
        }


        private static string GenerateCacheKey(List<KeyValuePair<CacheResources, string>> cacheKeyPairs)
        {
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append($"{App}");

            foreach (var cacheKeyItem in cacheKeyPairs)
            {
                keyBuilder.Append($":{cacheKeyItem.Key.GetEnumName()}:{cacheKeyItem.Value}");
            }

            return keyBuilder.ToString();
        }

      
        public async Task<T> GetResourceDataPerKeyValuePairsAsync<T>(List<KeyValuePair<CacheResources, string>> cacheKeyPairs)
        {
            var key = GenerateCacheKey(cacheKeyPairs);
            var value = await _cacheDb.StringGetAsync(key);

            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public async Task<bool> RemoveResourceDataPerKeyValuePairsAsync(List<KeyValuePair<CacheResources, string>> cacheKeyPairs)
        {
            var key = GenerateCacheKey(cacheKeyPairs);
            bool value = await _cacheDb.KeyExistsAsync(key);
            if (value)
            {
                await _cacheDb.KeyDeleteAsync(key);
                return true;
            }
            return false;
        }
    }

    public static class EnumExtensions
    {
        public static string? GetEnumName<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            return Enum.GetName(typeof(TEnum), value)?.ToLower();
        }
    }
}
