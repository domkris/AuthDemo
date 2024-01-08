using AuthDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthDemo.Domain
{
    public class AuthDemoDbContext: DbContext
    {
        public AuthDemoDbContext(DbContextOptions<AuthDemoDbContext> options) : base(options)
        {
        }

        public DbSet<Chore> Chores { get; set; }
    }

    internal class AuthDemoDbContextFactory : IDesignTimeDbContextFactory<AuthDemoDbContext>
    {
        public AuthDemoDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuthDemoDbContext>();
            optionsBuilder.UseSqlite(
                configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(AuthDemoDbContext).Assembly.FullName)
                );

            return new AuthDemoDbContext(optionsBuilder.Options);
        }
    }
}
