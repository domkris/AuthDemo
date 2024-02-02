namespace AuthDemo.Cache.Models
{
    public sealed class UserAgentInfo
    {
        public required string BrowserName { get; set; }
        public required string Version { get; set; }
        public required string Platform { get; set; }

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
