using AuthDemo.Infrastructure.Audit;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Infrastructure.Entities
{
    public class User : IdentityUser<long>, IAuditableEntity
    {
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }

        [Required]
        public virtual long RoleId { get; set; }
        public virtual Role? Role { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual long? CreatedById { get; set; }
        public virtual User? CreatedBy { get; set; }
        public virtual long? UpdatedById { get; set; }
        public virtual User? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
