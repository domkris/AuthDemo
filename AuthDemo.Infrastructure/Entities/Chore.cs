using AuthDemo.Infrastructure.Audit;
using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Infrastructure.Entities
{
    public class Chore : BaseEntity, IAuditableEntity
    {
        [MinLength(3)]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public long? UserAssigneeId { get; set; }
        public User? UserAssignee { get; set; }
        public bool IsFinished { get; set; }
        public bool IsApproved { get; set; }
        public long? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public long? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
