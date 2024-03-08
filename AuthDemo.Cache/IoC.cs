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
            string? env = Environment.GetEnvironmentVariable("DOCKER_COMPOSE");

            if(env == "true")
            {
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                {
                    return ConnectionMultiplexer.Connect(configuration.GetConnectionString("DockerRedis")!);
                });
            }
            else
            {
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                {
                    return ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!);
                });
            }
           
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton(provider =>
               provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        }
    }
}
