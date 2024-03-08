using static AuthDemo.Cache.Constants.CacheKeys;

namespace AuthDemo.Cache.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task<bool> SetDataAsync<T>(string key, T value);
        Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expiration);
        Task<bool> RemoveDataAsync(string key);

        Task<bool> RemoveResourcePerObjectIdAsync(CacheResources resource, string resourceId, string objectId);
        Task<bool> RemoveAllResourcesPerObjectIdAsync(CacheResources resource, string objectId);
        Task<IEnumerable<T>> GetAllResourcesPerObjectIdAsync<T>(CacheResources resource, string objectId);
        Task<T> GetResourcePerObjectIdAsync<T>(CacheResources resource, string resourceId, string objectId);
        Task<bool> SetResourceDataPerObjectIdAsync<T>(CacheResources resource, string resourceId, T resourceValue, string objectId, DateTimeOffset expiration);
    }
}
