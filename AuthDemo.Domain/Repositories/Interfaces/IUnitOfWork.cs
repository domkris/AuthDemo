namespace AuthDemo.Domain.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IChoresRepository Chores { get; }
        ITokensRepository Tokens { get; }
        Task<int> SaveAsync();
    }
}
