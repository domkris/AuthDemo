using AuthDemo.Infrastructure.LookupData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace AuthDemo.Security.Authorization
{
    internal static class AuthorizationConfiguration
    {
        public static void AddGlobalAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

                options.AddPolicy(AuthDemoPolicies.Roles.Admin, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, Roles.Administrator.GetValue());
                });

                options.AddPolicy(AuthDemoPolicies.Roles.Manager, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, Roles.Manager.GetValue());
                });

                options.AddPolicy(AuthDemoPolicies.Roles.AdminOrManager, policy =>
                {
                    policy.RequireClaim(
                        ClaimTypes.Role,
                        Roles.Administrator.GetValue(),
                        Roles.Manager.GetValue());
                });
            });
        }
       
    }

    public static class EnumExtensions
    {
        public static string GetName<T>(this T enumValue) where T : Enum
        {
               return Enum.GetName(typeof(T), enumValue)!;
        }

        public static string GetValue<T>(this T enumValue) where T : Enum
        {
               return enumValue.ToString("D");
        }
    }

}
