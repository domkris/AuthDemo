using static AuthDemo.Domain.Cache.CacheKeys;

namespace AuthDemo.Domain
{
    public interface ISystemCache
    {
        Task<T?> GetDataAsync<T>(string key);
        Task<bool> SetDataAsync<T>(string key, T value);
        Task<bool> SetDataAsync<T>(string key, T value, DateTime expiration);
        Task<bool> RemoveDataAsync(string key);

        Task<bool> RemoveResourcePerObjectIdAsync(CacheResources resource, string resourceId, string objectId);
        Task<bool> RemoveAllResourcesPerObjectIdAsync(CacheResources resource, string objectId);
        Task<IEnumerable<T>> GetAllResourcesPerObjectIdAsync<T>(CacheResources resource, string objectId);
        Task<T> GetResourcePerObjectIdAsync<T>(CacheResources resource, string resourceId, string objectId);
        Task<bool> SetResourceDataPerObjectIdAsync<T>(CacheResources resource, string resourceId, T resourceValue, string objectId, DateTime expiration);
    }
}
