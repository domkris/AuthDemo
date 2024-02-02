using AuthDemo.Domain.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AuthDemo.Domain
{
    public static class IoC
    {
        public static void RegisterDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!);
            });

            services.AddSingleton<ISystemCache, SystemCache>();
            services.AddSingleton(provider =>
               provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        }
    }
}
