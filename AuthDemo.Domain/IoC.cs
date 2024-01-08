using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthDemo.Infrastructure
{
    public static class IoC
    {
        public static void RegisterDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDemoDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
