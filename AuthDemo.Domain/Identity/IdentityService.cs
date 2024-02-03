using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthDemo.Domain.Identity
{
    internal class IdentityService<T, TKey> : IIdentityService<T, TKey> where T : class
    {
        private readonly UserManager<T> _userManager;
        private readonly SignInManager<T> _signInManager;
        public IdentityService(
            UserManager<T> userManager,
            SignInManager<T> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> ChangePasswordAsync(T user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> CreateAsync(T user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<T?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<T?> FindByIdAsync(TKey id)
        {
            if(id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _userManager.FindByIdAsync(id.ToString()!);
        }

        public async Task<IdentityResult> UpdateAsync(T user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> PasswordSignInAsync(T user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public IQueryable<T> GetAll()
        {
            return _userManager.Users;
        }

        public async Task<T?> FindByUserNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }
    }
}
