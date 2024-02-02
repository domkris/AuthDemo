using AuthDemo.Domain;
using AuthDemo.Domain.Cache.CacheObjects;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static AuthDemo.Domain.Cache.CacheKeys;

namespace AuthDemo.Security.Authentication
{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ISystemCache _systemCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtTokenGenerator(
            ISystemCache systemCache,
            IOptions<JwtSettings> jwtSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _systemCache = systemCache;
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
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
            var userAgent = _httpContextAccessor.HttpContext!.Request.Headers["Test"].ToString();

            UserToken cachedUserToken = new UserToken
            {
                UserId = user.Id.ToString(),
                TokenId = tokenGuid,
                Token = HashToken(result),
                TokenExpiration = tokenExpiration,
                TokenDuration = TimeSpan.FromMinutes(_jwtSettings.ExpirationInMinutes),
                UserAgentInfo = UserAgentInfo.Parse(userAgent) 
              
            };
            await _systemCache.SetResourceDataPerObjectIdAsync(CacheResources.UserToken, tokenGuid, cachedUserToken, user.Id.ToString(), tokenExpiration);

            return result;
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
