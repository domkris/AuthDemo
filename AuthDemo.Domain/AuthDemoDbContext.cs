using AuthDemo.Infrastructure.Audit;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthDemo.Infrastructure
{
    public class AuthDemoDbContext : IdentityDbContext<User, Role, long>
    {
        public AuthDemoDbContext(DbContextOptions<AuthDemoDbContext> options) : base(options)
        {
        }
    
        public DbSet<Chore> Chores { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // remove asp.net identity default many-to-many relationship between User and Role
            builder.Ignore<IdentityUserRole<long>>();
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

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

    // This is only used for migrations and dotnet ef database update
    // See https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-a-design-time-factory
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
