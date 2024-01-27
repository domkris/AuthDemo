using AuthDemo.Infrastructure.Audit;
using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Infrastructure.Entities
{
    public class Role : IdentityRole<long>
    {
        public virtual ICollection<User>? Users { get; set; }
    }
}
