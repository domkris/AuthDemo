using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthDemo.Infrastructure
{
    public static class IoC
    {
        public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext with Dependency Injection
            // See https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#using-dbcontext-with-dependency-injection
            services.AddDbContext<AuthDemoDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
