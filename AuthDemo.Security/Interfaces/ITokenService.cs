using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Security.Interfaces
{
    public interface ITokenService
    {
        Task<(string, string)> GenerateTokens(User user);
        Task<(string, string)> VerifyAndGenerateTokens(string accessToken, string refreshToken);
        Task<bool> IsAccessTokenCached(string tokenId, long userId);
        Task<bool> InvalidateUserTokens(long userId, string reasonOfRevoke);
        Task<bool> InvalidateUserTokensOnLogout(string accessTokenId, long userId, string reasonOfRevoke);
    }
}
