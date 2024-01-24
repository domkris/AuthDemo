using AuthDemo.Infrastructure.Audit;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Infrastructure.Entities
{
    public class User : IdentityUser<long>, IAuditableEntity
    {
        [Required]
        [MinLength(1)]
        public virtual required string FirstName { get; set; }

        [Required]
        [MinLength(1)]
        public virtual required string LastName { get; set; }
        [Required]
        public virtual long RoleId { get; set; }
        public virtual Role? Role { get; set; }
        public virtual bool IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
