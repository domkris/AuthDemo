using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AuthDemo.Domain.Identity
{
    internal class UserIdentityService : IdentityService<User, long>, IUserIdentityService
    {
        private readonly UserManager<User> _userManager;
        public UserIdentityService(UserManager<User> userManager, SignInManager<User> signInManager) : base(userManager, signInManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetCustomUniqueUserName(string firstName, string lastName)
        {
            var username = string.Concat(firstName, lastName).ToLower();

            var existingUser = await _userManager.Users
           .Where(user => user.FirstName == firstName && user.LastName == lastName)
           .OrderByDescending(user => user.UserName)
           .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                Match match = Regex.Match(existingUser.UserName!, @"\d+");
                int.TryParse(match.Value, out int suffix);
                if (suffix != 0)
                {
                    suffix++;
                    username = string.Concat(firstName, lastName, suffix).ToLower();
                }
            }

            return username;
        }
    }
}
