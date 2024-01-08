using AuthDemo.Infrastructure.Audit;

namespace AuthDemo.Infrastructure.Entities
{
    public class Chore : BaseEntity, IAuditableEntity
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
