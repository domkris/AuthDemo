using AuthDemo.Infrastructure.Audit;
using AuthDemo.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthDemo.Infrastructure
{
    public class AuthDemoDbContext(DbContextOptions<AuthDemoDbContext> options) : DbContext(options)
    {
        public DbSet<Chore> Chores { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditableEntity && (
                    e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((IAuditableEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                } else
                {
                    ((IAuditableEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
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
