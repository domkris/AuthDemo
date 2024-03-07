using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Domain.Repositories.Interfaces
{
    public interface ITokensRepository : IGenericRepository<Token, long>
    {
    }
}
