using AuthDemo.Domain.Utilities;

namespace AuthDemo.Domain.Cache.CacheObjects
{
    public sealed class UserAgentInfo
    {
        public required string BrowserName { get; set; }
        public required string Version { get; set; }
        public required string Platform { get; set; }

        public static UserAgentInfo Parse(string userAgentString)
        {
            return new UserAgentParser().Parse(userAgentString);
        }
    }

    public class UserToken
    {
        public required string UserId { get; set; }
        public required string TokenId { get; set; }
        public required DateTime TokenExpiration { get; set; }
        public required TimeSpan TokenDuration { get; set; }
        public required string Token { get; set; }
        public UserAgentInfo? UserAgentInfo { get; set; }
    }
}
