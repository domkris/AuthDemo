using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthDemo.Infrastructure.EntityTypeConfiguration
{
    internal sealed class AuditLog : IEntityTypeConfiguration<Audit.AuditLog>
    {
        public void Configure(EntityTypeBuilder<Audit.AuditLog> builder)
        {
            builder.HasIndex(entity => entity.EntityType);
            builder.HasIndex(entity => entity.EntityId);
        }
    }
}
