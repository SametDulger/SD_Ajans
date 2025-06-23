using SD_Ajans.Core.Entities;

namespace SD_Ajans.Core.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IGenericRepository<Manken> Mankens { get; }
        IGenericRepository<Organization> Organizations { get; }
        IGenericRepository<Assignment> Assignments { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<User> Users { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 