using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Security.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GenerateToken(User user);
        Task<bool> IsTokenCached(string tokenId, long userId);
    }
}
