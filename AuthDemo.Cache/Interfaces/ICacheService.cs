using static AuthDemo.Cache.Constants.CacheKeys;

namespace AuthDemo.Cache.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task<bool> SetDataAsync<T>(string key, T value);
        Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expiration);
        Task<bool> RemoveDataAsync(string key);

        Task<bool> RemoveAllResourcesPerIdAsync(CacheResources resource, string resourceId);
        Task<bool> RemoveResourceDataPerKeyValuePairsAsync(List<KeyValuePair<CacheResources, string>> cacheKeyPairs);
        Task<T> GetResourceDataPerKeyValuePairsAsync<T>(List<KeyValuePair<CacheResources, string>> cacheKeyPairs);
        Task<bool> SetResourceDataPerKeyValuePairsAsync<T>(List<KeyValuePair<CacheResources, string>> cacheKeyPairs, T cacheValue,  DateTimeOffset expiration);
    }
}
