using Domain.Entities;
using Domain.Models;

namespace Infrastructure.Repositories.Intf
{
    public interface IRepository : IBaseRepository<Model, Entity>
    {
    }
}
