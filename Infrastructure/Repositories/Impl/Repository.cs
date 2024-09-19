using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Repositories.Intf;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.Impl
{
    public class Repository : BaseRepository<Model, Entity>, IRepository
    {
        public Repository(SqlConnection connection, IDbTransaction transaction, IMapper mapper) : base(connection, transaction, mapper)
        {
        }
    }
}
