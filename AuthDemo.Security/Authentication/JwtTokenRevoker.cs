using AuthDemo.Domain;
using AuthDemo.Domain.Cache.CacheObjects;
using static AuthDemo.Domain.Cache.CacheKeys;

namespace AuthDemo.Security.Authentication
{
    public interface IJwtTokenRevoker
    {
        Task<bool> IsTokenInCache(string tokenId, long userId);
    }
    public class JwtTokenRevoker : IJwtTokenRevoker
    {
        private readonly ISystemCache _systemCache;
        public JwtTokenRevoker(
            ISystemCache systemCache)
        {
            _systemCache = systemCache;
        }
        public async Task<bool> IsTokenInCache(string tokenId, long userId)
        {
            var result = await _systemCache.GetResourcePerObjectIdAsync<UserToken>(CacheResources.UserToken, tokenId, userId.ToString());
            if(result == null)
            {
                return false;
            }
            return true;
        }
    }
}
