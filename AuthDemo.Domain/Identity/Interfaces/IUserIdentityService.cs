using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Domain.Identity.Interfaces
{
    public interface IUserIdentityService : IIdentityService<User, long>
    {
        Task<string> GetCustomUniqueUserName(string firstName, string lastName);
    }
}
