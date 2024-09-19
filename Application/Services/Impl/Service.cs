using Application.Services.Intf;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Repositories.Intf;
using Infrastructure.UnitOfWork.Intf;

namespace Application.Services.Impl
{
    public class Service : BaseService<Model, Entity, IRepository>, IService
    {
        private readonly IUnitOfWork _uow;

        public Service(IUnitOfWork uow) : base(uow, uow.Repository)
        {
            _uow = uow;
        }
    }
}
