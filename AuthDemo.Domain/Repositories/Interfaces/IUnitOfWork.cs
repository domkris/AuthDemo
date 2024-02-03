namespace AuthDemo.Domain.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IChoresRepository Chores { get; }

        Task<int> SaveAsync();

        // int Save();

    }
}
