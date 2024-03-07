
using AuthDemo.Cache.Interfaces;
using AuthDemo.Cache.Models;
using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Domain.Repositories.Interfaces;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyCSharp.HttpUserAgentParser;
using MyCSharp.HttpUserAgentParser.Providers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static AuthDemo.Cache.Constants.CacheKeys;

namespace AuthDemo.Security.Authentication
{
    internal class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly ICacheService _cacheService;
        private readonly IHttpUserAgentParserProvider _parser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserIdentityService _userIdentityService;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IOptions<JwtSettings> jwtSettings,
            IHttpUserAgentParserProvider parser,
            IHttpContextAccessor httpContextAccessor,
            IUserIdentityService userIdentityService,
            TokenValidationParameters tokenValidationParameters)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _jwtSettings = jwtSettings.Value;
            _parser = parser;
            _httpContextAccessor = httpContextAccessor;
            _userIdentityService = userIdentityService;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<bool> IsAccessTokenCached(string tokenId, long userId)
        {
            var result = await _cacheService.GetResourcePerObjectIdAsync<AccessToken>(CacheResources.AccessToken, tokenId, userId.ToString());
            if (result == null)
            {
                return false;
            }
            return true;
        }

       
        public async Task<(string, string)> GenerateTokens(User user)
        {
            string accessTokenGuid = Guid.NewGuid().ToString();
            Token refreshToken = await GenerateRefreshTokenAsync(user, accessTokenGuid);
            (var acessTokenResult, AccessToken accessToken) = GenerateAccessToken(user, refreshToken.RefreshToken, accessTokenGuid);
    
            await _cacheService.SetResourceDataPerObjectIdAsync(CacheResources.AccessToken, accessToken.TokenId, accessToken, user.Id.ToString(), accessToken.TokenExpiration);

            return (acessTokenResult, refreshToken.RefreshToken);
        }

        public async Task<(string, string)> VerifyAndGenerateTokens(string accessToken, string refreshToken)
        {
            var emptyResult = (string.Empty, string.Empty);

            try
            {
                ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(accessToken, _tokenValidationParameters, out SecurityToken validatedAccessToken);

                var accessTokenId = validatedAccessToken.Id;
                long.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedUserId);

                if (string.IsNullOrEmpty(accessTokenId))
                {
                    return emptyResult;
                }

                var currentTime = DateTime.Now;
                Token? storedRefreshToken = await _unitOfWork.Tokens.GetAll()
                    .Where(token => token.Expires > currentTime)
                    .Where(token => token.Revoked == null)
                    .Where(token => token.ReplacedByRefreshToken == null)
                    .FirstOrDefaultAsync(token => token.RefreshToken == refreshToken);

                if (storedRefreshToken == null)
                {
                    return emptyResult;
                }

                if(storedRefreshToken.UserId != loggedUserId)
                {
                    return emptyResult;
                }

                if(storedRefreshToken.JwtAccessTokenId != accessTokenId)
                {
                    return emptyResult;
                }

                // if accessToken is not expired, expire it
                if(await IsAccessTokenCached(accessTokenId, loggedUserId))
                {
                    await _cacheService.RemoveResourcePerObjectIdAsync(CacheResources.AccessToken, accessTokenId, loggedUserId.ToString());
                }

                User? user = await _userIdentityService
                    .GetAll()
                    .Where(user => user.IsActive == true)
                    .FirstOrDefaultAsync(user => user.Id == loggedUserId);

                if(user == null)
                {
                    return emptyResult;
                }

                (string acessTokenResult, string refreshTokenResult) = await GenerateTokens(user);

                storedRefreshToken.ReplacedByRefreshToken = refreshTokenResult;
                await _unitOfWork.SaveAsync();

                return (acessTokenResult, refreshTokenResult);


            }
            catch (Exception)
            {

                return emptyResult;
            }

            return emptyResult;

        }

        private (string, AccessToken) GenerateAccessToken(User user, string? refreshToken, string accessTokenGuid)
        {
            SigningCredentials signingCredentials = new SigningCredentials(
               new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
               SecurityAlgorithms.HmacSha256);

            var accessTokenExpiration = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiration);

            var claims = new[]
          {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, accessTokenGuid),
            };

            var securityKey = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: accessTokenExpiration,
                signingCredentials: signingCredentials
            );

            string result = new JwtSecurityTokenHandler().WriteToken(securityKey);
            var userAgentString = _httpContextAccessor.HttpContext!.Request.Headers["User-Agent"].ToString();

            AccessToken token = new AccessToken
            {
                UserId = user.Id.ToString(),
                TokenId = accessTokenGuid,
                RefreshToken = refreshToken,
                TokenExpiration = accessTokenExpiration,
                TokenDuration = TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpiration),
                UserAgentInfo = Parse(userAgentString)

            };

            return (result, token);
        }

        

        private async Task<Token> GenerateRefreshTokenAsync(User user, string accessTokenGuid)
        {
            const int tokenLength = 64;
            var refreshTokenExpiration = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiration);
            string refreshTokenRandomString = GenerateRandomString(tokenLength);

            Token token = new Token
            {
                UserId = user.Id,
                User = user,
                JwtAccessTokenId = accessTokenGuid,
                RefreshToken = refreshTokenRandomString,
                Expires = refreshTokenExpiration,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.Tokens.Add(token);
            await _unitOfWork.SaveAsync();

            return token;
        }

        private static string GenerateRandomString(int length)
        {
            /*
            byte[] randomNumber = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomNumber);
            }

            // Convert the random bytes to a base64-encoded string
            string result = Convert.ToBase64String(randomNumber);

            // Remove any invalid characters from the base64 string
            result = result.Replace("/", "").Replace("+", "")
                                       .Replace("=", "").Replace("\\", "");
            return result;
            */

            var randomNUmber = new byte[length];
            using var numGenerator = RandomNumberGenerator.Create();
            numGenerator.GetBytes(randomNUmber);

            return Convert.ToBase64String(randomNUmber);
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

        
        public async Task<bool> InvalidateUserTokens(long userId, string reasonOfRevoke)
        {
            var accessTokenInvalidationResult = await _cacheService.RemoveAllResourcesPerObjectIdAsync(CacheResources.AccessToken, userId.ToString());

            List<Token>? refreshTokens = await _unitOfWork.Tokens.GetAll()
                .Where(token => token.UserId == userId)
                .Where(token => token.IsActive == true)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.Revoked = DateTime.UtcNow;
                token.ReasonRevoked = reasonOfRevoke;
            }

            await _unitOfWork.SaveAsync();
            return accessTokenInvalidationResult;
        }

        public async Task<bool> InvalidateUserTokensOnLogout(string accessTokenId, long userId, string reasonOfRevoke)
        {
            AccessToken accessToken = await _cacheService.GetResourcePerObjectIdAsync<AccessToken>(CacheResources.AccessToken, userId.ToString(), accessTokenId);

            Token? refreshToken = await _unitOfWork.Tokens.GetAll()
                .FirstOrDefaultAsync(token => token.RefreshToken == accessToken.RefreshToken);

            if(refreshToken != null)
            {
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.ReasonRevoked = reasonOfRevoke;
            }
            await _unitOfWork.SaveAsync();

            return await _cacheService.RemoveResourcePerObjectIdAsync(CacheResources.AccessToken, userId.ToString(), accessTokenId);
        }
    }
} 
