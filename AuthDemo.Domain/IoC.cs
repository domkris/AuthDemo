using AuthDemo.Domain.Identity;
using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Domain.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthDemo.Domain
{
    public static class IoC
    {
        public static void RegisterDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserIdentityService, UserIdentityService>();
        }
    }
}
