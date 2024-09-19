using Domain.Entities;
using Domain.Models;
using Infrastructure.Repositories.Intf;
using Infrastructure.UnitOfWork.Intf;
using System.Linq.Expressions;

namespace Application.Services.Impl
{
    public abstract class BaseService<TModel, TEntity, TIRepository>
        where TModel : BaseModel
        where TEntity : BaseEntity
        where TIRepository : IBaseRepository<TModel, TEntity>
    {
        private readonly IUnitOfWork _uow;
        private readonly TIRepository _repository;

        public BaseService(IUnitOfWork uow, TIRepository repository)
        {
            _uow = uow;
            _repository = repository;
        }

        public async Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            try
            {
                return await _repository.GetAllAsync(predicate);
            }
            catch
            {
                throw;
            }
        }

        public async Task<TModel> GetAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            try
            {
                return await _repository.GetAsync(predicate);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> CreateAsync(TModel model)
        {
            try
            {
                return await _repository.CreateAsync(model);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> UpdateAsync(TModel model, Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _repository.UpdateAsync(model, predicate);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _repository.DeleteAsync(predicate);
            }
            catch
            {
                throw;
            }
        }

        public void Commit()
        {
            _uow.Commit();
        }

        public void Dispose()
        {
            _uow.Dispose();
        }
    }
}
