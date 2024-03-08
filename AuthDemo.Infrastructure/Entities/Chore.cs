using AuthDemo.Infrastructure.Audit;
using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Infrastructure.Entities
{
    public class Chore : BaseEntity, IAuditableEntity
    {
        [MinLength(3)]
        public required string Title { get; set; }
        public virtual string? Description { get; set; }
        public virtual long? UserAssigneeId { get; set; }
        public virtual User? UserAssignee { get; set; }
        public virtual bool IsFinished { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual long? CreatedById { get; set; }
        public virtual User? CreatedBy { get; set; }
        public virtual long? UpdatedById { get; set; }
        public virtual User? UpdatedBy { get; set; }
        public virtual DateTimeOffset? CreatedAt { get; set; }
        public virtual DateTimeOffset? UpdatedAt { get; set; }
    }
}
