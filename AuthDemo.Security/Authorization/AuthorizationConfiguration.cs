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
                    policy.RequireRole(
                        Enum.GetName(typeof(Roles), Roles.Administrator)!);
                });

                options.AddPolicy(AuthDemoPolicies.Roles.AdminAndManager, policy =>
                {
                    policy.RequireRole(
                        Enum.GetName(typeof(Roles), Roles.Manager)!,
                        Enum.GetName(typeof(Roles), Roles.Administrator)!);
                });
            });
        }
    }
}
