using AuthDemo.Infrastructure.LookupData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

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
                    policy.RequireRole(Roles.Administrator.GetValue());
                });

                options.AddPolicy(AuthDemoPolicies.Roles.AdminAndManager, policy =>
                {
                    policy.RequireRole(
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
