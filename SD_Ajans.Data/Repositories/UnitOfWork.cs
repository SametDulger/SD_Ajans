using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SD_Ajans.Core.Repositories;

namespace SD_Ajans.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<T>(_context);
            }
            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.DisposeAsync().GetAwaiter().GetResult();
                _transaction = null;
            }
            _context?.Dispose();
        }

        public IGenericRepository<SD_Ajans.Core.Entities.Manken> Mankens => Repository<SD_Ajans.Core.Entities.Manken>();
        public IGenericRepository<SD_Ajans.Core.Entities.Organization> Organizations => Repository<SD_Ajans.Core.Entities.Organization>();
        public IGenericRepository<SD_Ajans.Core.Entities.Assignment> Assignments => Repository<SD_Ajans.Core.Entities.Assignment>();
        public IGenericRepository<SD_Ajans.Core.Entities.Payment> Payments => Repository<SD_Ajans.Core.Entities.Payment>();
        public IGenericRepository<SD_Ajans.Core.Entities.User> Users => Repository<SD_Ajans.Core.Entities.User>();
    }
} 