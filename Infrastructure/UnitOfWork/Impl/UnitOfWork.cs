using Infrastructure.Repositories.Intf;
using Infrastructure.UnitOfWork.Intf;
using System.Data;

namespace Infrastructure.UnitOfWork.Impl
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDbTransaction _transaction;
        public IRepository Repository { get; }

        public UnitOfWork(IDbTransaction transaction, IRepository repository)
        {
            _transaction = transaction;
            Repository = repository;
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }

        public void Dispose()
        {
            _transaction.Connection?.Close();
            _transaction.Connection?.Dispose();
            _transaction.Dispose();
        }
    }
}
