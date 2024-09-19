using Domain.Entities;
using Domain.Models;
using System.Linq.Expressions;


namespace Application.Services.Intf
{
    public interface IBaseService<TModel, TEntity>
        where TModel : BaseModel
        where TEntity : BaseEntity
    {
        Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<TModel> GetAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<int> CreateAsync(TModel model);
        Task<int> UpdateAsync(TModel model, Expression<Func<TEntity, bool>> predicate);
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        void Commit();
        void Dispose();
    }
}
