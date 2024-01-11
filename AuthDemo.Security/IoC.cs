using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Authentication;
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

            services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AuthDemoDbContext>();

            return services;
        }
    }
}
