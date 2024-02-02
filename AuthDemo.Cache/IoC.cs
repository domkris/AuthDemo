using AuthDemo.Cache.Interfaces;
using AuthDemo.Cache.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace AuthDemo.Cache
{
    public static class IoC
    {
        public static void RegisterCacheServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!);
            });

            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton(provider =>
               provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        }
    }
}
