using AuthDemo.Infrastructure.Audit;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Infrastructure.Entities
{
    public class User : IdentityUser<long>, IAuditableEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public long RoleId { get; set; }
        public Role? Role { get; set; }

        public bool IsActive { get; set; }
        public long? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public long? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
