using AuthDemo.Domain.Repositories.Interfaces;
using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Domain.Repositories
{
    public class TokensRepository : GenericRepository<Token, long>, ITokensRepository
    {
        public TokensRepository(AuthDemoDbContext dbContext) : base(dbContext)
        {

        }
    }
}
