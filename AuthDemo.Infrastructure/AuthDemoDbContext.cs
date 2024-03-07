using AuthDemo.Infrastructure.Audit;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthDemo.Infrastructure
{
    public class AuthDemoDbContext : IdentityDbContext<User, Role, long>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        public AuthDemoDbContext(
            IHttpContextAccessor? httpContextAccessor,
            DbContextOptions<AuthDemoDbContext> options) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    
        public DbSet<Chore> Chores { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditableEntity && (
                    e.State == EntityState.Added || e.State == EntityState.Modified));

            long userId = GetSystemOrLoggedInUserId();

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((IAuditableEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                    ((IAuditableEntity)entityEntry.Entity).CreatedById = userId;
                }
                else
                {
                    ((IAuditableEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
                    ((IAuditableEntity)entityEntry.Entity).UpdatedById = userId;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        private long GetSystemOrLoggedInUserId()
        {
            if (IsAnonymousEndpoint())
            {
                return 1L;
            }
            else
            {
                string? userId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(userId, out long id))
                {
                    throw new ArgumentException($"Invalid input for {userId}. Unable to parse as a valid long value");
                }
                return id;
            }
        }

        private bool IsAnonymousEndpoint()
        {
            if (_httpContextAccessor?.HttpContext?.GetEndpoint()?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null) 
            {
                return true;
            }
            return false;
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
            return new AuthDemoDbContext(null, optionsBuilder.Options);
        }
    }
}
