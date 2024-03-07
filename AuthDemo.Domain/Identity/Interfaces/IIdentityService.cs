using Microsoft.AspNetCore.Identity;

namespace AuthDemo.Domain.Identity.Interfaces
{
    public interface IIdentityService<T, TKey> where T : class
    {
        IQueryable<T> GetAll();
        Task<T?> FindByEmailAsync(string email);
        Task<T?> FindByUserNameAsync(string username);
        Task<T?> FindByIdAsync(TKey id);
        Task<IdentityResult> CreateAsync(T user, string password);
        Task<IdentityResult> UpdateAsync(T user);
        Task<IdentityResult> ChangePasswordAsync(T user, string currentPassword, string newPassword);
        Task<SignInResult> PasswordSignInAsync(T user, string password, bool isPersistent, bool lockoutOnFailure);
    }
}
