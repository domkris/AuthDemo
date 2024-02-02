
using AuthDemo.Cache.Interfaces;
using AuthDemo.Cache.Models;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyCSharp.HttpUserAgentParser.Providers;
using static AuthDemo.Cache.Constants.CacheKeys;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyCSharp.HttpUserAgentParser;
using System.Security.Cryptography;

namespace AuthDemo.Security.Authentication
{
    internal class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ICacheService _cacheService;
        private readonly IHttpUserAgentParserProvider _parser;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenService(
            ICacheService cacheService,
            IOptions<JwtSettings> jwtSettings,
            IHttpUserAgentParserProvider parser,
            IHttpContextAccessor httpContextAccessor)
        {
            _cacheService = cacheService;
            _jwtSettings = jwtSettings.Value;
            _parser = parser;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IsTokenCached(string tokenId, long userId)
        {
            var result = await _cacheService.GetResourcePerObjectIdAsync<UserToken>(CacheResources.UserToken, tokenId, userId.ToString());
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public async Task<string> GenerateToken(User user)
        {
            SigningCredentials signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256);

            var tokenGuid = Guid.NewGuid().ToString();
            var tokenExpiration = DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, tokenGuid),
            };

            var securityKey = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: signingCredentials
            );

            string result = new JwtSecurityTokenHandler().WriteToken(securityKey);
            var userAgentString = _httpContextAccessor.HttpContext!.Request.Headers["User-Agent"].ToString();

            UserToken cachedUserToken = new UserToken
            {
                UserId = user.Id.ToString(),
                TokenId = tokenGuid,
                Token = HashToken(result),
                TokenExpiration = tokenExpiration,
                TokenDuration = TimeSpan.FromMinutes(_jwtSettings.ExpirationInMinutes),
                UserAgentInfo = Parse(userAgentString)

            };
            await _cacheService.SetResourceDataPerObjectIdAsync(CacheResources.UserToken, tokenGuid, cachedUserToken, user.Id.ToString(), tokenExpiration);

            return result;
        }

        private UserAgentInfo Parse(string? userAgentString)
        {
            if (string.IsNullOrEmpty(userAgentString))
            {
                // Handle empty or null user-agent string
                return new UserAgentInfo
                {
                    BrowserName = "Unknown",
                    Version = "Unknown",
                    Platform = "Unknown"
                };
            }

            HttpUserAgentInformation info = _parser.Parse(userAgentString);

            return new UserAgentInfo
            {
                BrowserName = info.Name ?? "Unknown",
                Version = info.Version ?? "Unknown",
                Platform = info.Platform?.Name ?? "Unknown"
            };
        }

        private string HashToken(string token)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            using HMACSHA256 hmacSha256 = new(keyBytes);
            byte[] hashedBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private static bool CompareHashes(string hash1, string hash2)
        {
            return string.Equals(hash1, hash2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
