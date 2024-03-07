using AuthDemo.Domain.Repositories;
using AuthDemo.Domain.Repositories.Interfaces;
using AuthDemo.Infrastructure;

namespace AuthDemo.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDemoDbContext _dbContext;
        public UnitOfWork(AuthDemoDbContext dbContext)
        {
            _dbContext = dbContext;
            Chores = new ChoresRepository(_dbContext);
            Tokens = new TokensRepository(_dbContext);
        }

        public IChoresRepository Chores { get; }
        public ITokensRepository Tokens { get; }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveAsync() 
        {
            return await _dbContext.SaveChangesAsync(); 
        }
    }
}
