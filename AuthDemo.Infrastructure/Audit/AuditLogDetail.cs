using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Infrastructure.Audit
{
    public class AuditLogDetail: BaseEntity
    {
        public required long AuditLogId { get; set; }
        public required AuditLog AuditLog { get; set; }
        public required string Property { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
