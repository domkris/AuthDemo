using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Authentication;
using AuthDemo.Security.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthDemo.Security
{
    public static class IoC
    {
        public static IServiceCollection RegisterSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.AddSingleton<JwtTokenGenerator>();

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
            });

            // services.AddIdentity internally sets cookie authentication handler as default
            //and therefore we will not be able to set JwtBearer as default but have to specify it explicitly in every controller in authorize attribute
            // so we will create a default policy in AddGlobalAuthorizationPolicies that will use JwtBearer authentication scheme
            services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AuthDemoDbContext>();

            services.AddGlobalAuthorizationPolicies();

            return services;
        }
    }
}
