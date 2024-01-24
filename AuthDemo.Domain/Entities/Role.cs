using AuthDemo.Infrastructure.Audit;
using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Infrastructure.Entities
{
    public class Role : IdentityRole<long>, IAuditableEntity
    {
        public virtual ICollection<User>? Users { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
