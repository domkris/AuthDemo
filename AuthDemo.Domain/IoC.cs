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
            /*
            services.AddSingleton<SystemCache>();
            services.AddSingleton(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

            services.AddScoped<SystemCache2>();
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

            services.AddScoped(provider =>
                provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = CacheKeys.App;
            });
            */
        }
    }
}
