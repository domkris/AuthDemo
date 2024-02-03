using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Domain.Identity
{
    internal class UserIdentityService : IdentityService<User, long>, IUserIdentityService
    {
        public UserIdentityService(UserManager<User> userManager, SignInManager<User> signInManager) : base(userManager, signInManager)
        {

        }
    }
}
