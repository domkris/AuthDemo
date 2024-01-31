using AuthDemo.Domain;
using AuthDemo.Domain.Cache.CacheObjects;
using AuthDemo.Infrastructure.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml;
using static AuthDemo.Domain.Cache.CacheKeys;

namespace AuthDemo.Security.Authentication
{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ISystemCache _systemCache;
        public JwtTokenGenerator(
            ISystemCache systemCache,
            IOptions<JwtSettings> jwtSettings)
        {
            _systemCache = systemCache;
            _jwtSettings = jwtSettings.Value;
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

            UserToken cachedUserToken = new UserToken
            {
                UserId = user.Id.ToString(),
                TokenId = tokenGuid,
                Token = result,
                TokenExpiration = tokenExpiration,
                TokenDuration = TimeSpan.FromMinutes(_jwtSettings.ExpirationInMinutes),
            };
            await _systemCache.SetResourceDataPerObjectIdAsync(CacheResources.UserToken, tokenGuid, cachedUserToken, user.Id.ToString(), tokenExpiration);

            return result;
        }
    }
}
