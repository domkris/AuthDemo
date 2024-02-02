using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Authentication;
using AuthDemo.Security.Authorization;
using AuthDemo.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MyCSharp.HttpUserAgentParser.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthDemo.Security
{
    public static class IoC
    {
        public static IServiceCollection RegisterSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpUserAgentParser();
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                configureOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions =>
            {
                string? secret = configuration.GetSection(nameof(JwtSettings))["Secret"];

                if(string.IsNullOrEmpty(secret))
                {
                    throw new Exception("JwtSettings.Secret is not configured.");
                }

                var secretKey = Encoding.UTF8.GetBytes(secret);

                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetSection(nameof(JwtSettings))["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration.GetSection(nameof(JwtSettings))["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                };
                var tokenService = services.BuildServiceProvider().GetRequiredService<IJwtTokenService>();
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                   OnTokenValidated = context => {
                       long.TryParse(context.Principal.FindFirstValue(ClaimTypes.NameIdentifier), out long userId);
                       string? tokenId = context.Principal.FindFirstValue(JwtRegisteredClaimNames.Jti);

                       if (string.IsNullOrEmpty(tokenId))
                       {
                           throw new SecurityTokenException($"Claim of type JwtRegisteredClaimNames.Jti is missing");
                       }

                       if(userId == 0) 
                       {                            
                           throw new SecurityTokenException($"Claim of type ClaimTypes.NameIdentifier is missing or has invalid value");
                       }

                       var result = tokenService.IsTokenCached(tokenId, userId).Result;
                       if (!result)
                       {
                           // each generated token must be in cache! If it is not it means we removed it from cache and therefore it is not valid anymore
                           context.Fail("Unauthorized");
                       }
                       return Task.CompletedTask;
                   }
                };
            });

            // services.AddIdentity internally sets cookie authentication handler as default
            //and therefore we will not be able to set JwtBearer as default but have to specify it explicitly in every controller in authorize attribute
            // so we will create a default policy in AddGlobalAuthorizationPolicies that will use JwtBearer authentication scheme
            services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;
            })
            //.AddUserStore<UserStore<User, Role, AuthDemoDbContext, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityUserToken<long>, IdentityRoleClaim<long>>>()
            //.AddRoleStore<RoleStore<Role, AuthDemoDbContext, long, IdentityUserRole<long>, IdentityRoleClaim<long>>>()
            .AddEntityFrameworkStores<AuthDemoDbContext>()
            .AddSignInManager<SignInManager<User>>();

            services.AddGlobalAuthorizationPolicies();

            return services;
        }
    }
}
