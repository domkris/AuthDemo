using AuthDemo.Infrastructure.Attributes;
using AuthDemo.Infrastructure.Audit;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Security.Claims;

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

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditLogDetail> AuditLogDetails { get; set; }
        public DbSet<Chore> Chores { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result = 0;
            try
            {
                long userId = GetSystemOrLoggedInUserId();

                List<EntityEntry> entriesToCreate = GetEntries(EntityState.Added);
                List<EntityEntry> entriesToModify = GetEntries(EntityState.Modified);
                List<EntityEntry> entriesToDelete = GetEntries(EntityState.Deleted);

                result += await UpdateAddedEntriesAndSaveToDbAsync(entriesToCreate, userId, cancellationToken);
                UpdateModifiedEntriesAsync(entriesToModify, userId, cancellationToken);


                if (entriesToCreate.Count > 0)
                {
                    result += await PerformAudit(EntityState.Added, entriesToCreate, userId, cancellationToken);
                }

                if (entriesToModify.Count > 0)
                {
                    result += await PerformAudit(EntityState.Modified, entriesToModify, userId, cancellationToken);
                }

                if (entriesToDelete.Count > 0)
                {
                    result += await PerformAudit(EntityState.Deleted, entriesToDelete, userId, cancellationToken);
                }

                return result;
                
            }

            catch (Exception)
            {

                throw;
            }
        }

        private List<EntityEntry> GetEntries(EntityState state)
        {
            return ChangeTracker
              .Entries()
              .Where(e => e.Entity is IAuditableEntity
                  && (e.State == state))
              .ToList();
        }

        private async Task<int> PerformAudit(EntityState state, List<EntityEntry> entries, long userId, CancellationToken cancellationToken)
        {
            foreach (var entityEntry in entries)
            {
                if (!IsExcludedFromAuditLog(entityEntry))
                {
                    await CreateAuditLog(state, entityEntry, userId);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateModifiedEntriesAsync(List<EntityEntry> entries, long userId, CancellationToken cancellationToken)
        {
            if(entries.Count == 0)
            {
                return;
            }

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State != EntityState.Modified)
                {
                    throw new InvalidOperationException("All entries must be in status Modified");
                }
                UpdateEntity(entityEntry, userId);

            }
        }

        private async Task<int> UpdateAddedEntriesAndSaveToDbAsync(List<EntityEntry> entries, long userId, CancellationToken cancellationToken)
        {
            if (entries.Count == 0)
            {
                return 0;
            }

            foreach (var entityEntry in entries)
            {
                if(entityEntry.State != EntityState.Added)
                {
                    throw new InvalidOperationException("All entries must be in status Added");
                }
                UpdateEntity(entityEntry, userId);

            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        private static bool IsExcludedFromAuditLog(EntityEntry entityEntry)
        {
            Type type = entityEntry.Entity.GetType();
            return type.GetCustomAttribute<ExcludeFromAuditLogAttribute>() != null;
        }

        private async Task CreateAuditLog(EntityState state, EntityEntry entityEntry, long userId)
        {
            AuditLog auditLog = new()
            {
                Action = state.ToString(),
                CreatedAt = DateTimeOffset.UtcNow,
                UserId = userId,
                EntityType = entityEntry.Entity.GetType().Name,
                EntityId = (long)entityEntry.Property("Id")!.CurrentValue!
            };

            switch (state)
            {
                case EntityState.Deleted:
                    break;
                case EntityState.Modified:
                    CreateAuditLogDetailsOnModify(auditLog, entityEntry);
                    break;
                case EntityState.Added:
                    CreateAuditLogDetailsOnCreate(auditLog, entityEntry);
                    break;
                default:
                    throw new InvalidOperationException("Wrong EntityEntry state");
            }

            await AuditLogs.AddAsync(auditLog);
        }

        private static void CreateAuditLogDetailsOnCreate(AuditLog auditLog, EntityEntry entityEntry)
        {
            List<AuditLogDetail> auditLogDetails = new();
            foreach (var property in entityEntry.OriginalValues.Properties)
            {
                string? newValue = entityEntry.CurrentValues[property]?.ToString();
                string? oldValue = null;

                AuditLogDetail auditLogDetail = new()
                {
                    AuditLog = auditLog,
                    AuditLogId = auditLog.Id,
                    Property = property.Name,
                    OldValue = oldValue,
                    NewValue = newValue

                };
                auditLogDetails.Add(auditLogDetail);

            }
            auditLog.AuditLogDetails = auditLogDetails;
        }

        private static void CreateAuditLogDetailsOnModify(AuditLog auditLog, EntityEntry entityEntry)
        {
            List<AuditLogDetail> auditLogDetails = new();
            foreach (var property in entityEntry.OriginalValues.Properties)
            {
                string? newValue = entityEntry.CurrentValues[property]?.ToString();
                string? oldValue = entityEntry.OriginalValues[property]?.ToString();

                if (Equals(newValue, oldValue))
                {
                    continue;
                }

                AuditLogDetail auditLogDetail = new()
                {
                    AuditLog = auditLog,
                    AuditLogId = auditLog.Id,
                    Property = property.Name,
                    OldValue = oldValue,
                    NewValue = newValue

                };
                auditLogDetails.Add(auditLogDetail);

            }
            auditLog.AuditLogDetails = auditLogDetails;
        }

        private static void UpdateEntity(EntityEntry entityEntry, long userId)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((IAuditableEntity)entityEntry.Entity).CreatedAt = DateTimeOffset.UtcNow;
                ((IAuditableEntity)entityEntry.Entity).CreatedById = userId;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                ((IAuditableEntity)entityEntry.Entity).UpdatedAt = DateTimeOffset.UtcNow;
                ((IAuditableEntity)entityEntry.Entity).UpdatedById = userId;
            }
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
                .AddJsonFile("appsettings.migrations.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuthDemoDbContext>();

            optionsBuilder.UseNpgsql(
                 configuration.GetConnectionString("DockerConnectionPostgres"),
                 builder => builder.MigrationsAssembly(typeof(AuthDemoDbContext).Assembly.FullName));

            return new AuthDemoDbContext(null, optionsBuilder.Options);
        }
    }
}
