using Infrastructure.Repositories.Intf;

namespace Infrastructure.UnitOfWork.Intf
{
    public interface IUnitOfWork
    {
        IRepository Repository { get; }

        void Commit();
        void Dispose();
    }
}
