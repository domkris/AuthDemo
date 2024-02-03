using AuthDemo.Domain.Repositories.Interfaces;
using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;

namespace AuthDemo.Domain.Repositories
{
    public class ChoresRepository : GenericRepository<Chore, long>, IChoresRepository
    {
        public ChoresRepository(AuthDemoDbContext dbContext): base(dbContext)
        {
            
        }
    }
}
