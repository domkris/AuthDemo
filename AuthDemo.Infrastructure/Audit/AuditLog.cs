using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Infrastructure.Audit
{
    public class AuditLog : BaseEntity
    {
        public long? UserId { get; set; }
        public User? User { get; set; }
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
        public string? Action { get; set; }
        public required DateTimeOffset CreatedAt { get; set; }
        public virtual ICollection<AuditLogDetail> AuditLogDetails { get; set; } = new HashSet<AuditLogDetail>();
    }
}
