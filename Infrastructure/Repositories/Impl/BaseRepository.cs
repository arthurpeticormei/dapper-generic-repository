using AutoMapper;
using Dapper;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Config;
using Infrastructure.Repositories.Intf;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Repositories.Impl
{
    public abstract class BaseRepository<TModel, TEntity> : IBaseRepository<TModel, TEntity>
        where TModel : BaseModel
        where TEntity : BaseEntity
    {
        private readonly SqlConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly IMapper _mapper;

        public BaseRepository(SqlConnection connection, IDbTransaction transaction, IMapper mapper)
        {
            _connection = connection;
            _transaction = transaction;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            IEnumerable<TEntity> entities = null;
            Type entityType = typeof(TEntity);

            try
            {
                string query = string.Format(@"SELECT * FROM {0} {1}", GetTableName(entityType), GetFilter(predicate));

                entities = await _connection.QueryAsync<TEntity>(query, transaction: _transaction);
            }
            catch
            {
                throw new Exception(@$"Could not list '{entityType.Name}' records.");
            }

            return _mapper.Map<IEnumerable<TModel>>(entities);
        }

        public async Task<TModel> GetAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            IEnumerable<TEntity> entities = null;
            Type entityType = typeof(TEntity);

            try
            {
                string query = string.Format(@"SELECT * FROM {0} {1}", GetTableName(entityType), GetFilter(predicate));

                entities = await _connection.QueryAsync<TEntity>(query, transaction: _transaction);
            }
            catch
            {
                throw new Exception(@$"Could not get '{entityType.Name}' record.");
            }

            return _mapper.Map<TModel>(entities.FirstOrDefault());
        }

        public async Task<int> CreateAsync(TModel model)
        {
            int rows = 0;
            Type entityType = typeof(TEntity);

            try
            {
                string columns = string.Join(", ", entityType.GetProperties().Select(p => p.Name));
                string values = string.Join(", ", entityType.GetProperties().Select(p => @$"@{p.Name}"));
                string query = string.Format(@"INSERT INTO {0} ({1}) VALUES ({2})", GetTableName(entityType), columns, values);

                TEntity entity = _mapper.Map<TEntity>(model);

                rows = await _connection.ExecuteAsync(query, entity, transaction: _transaction);
            }
            catch
            {
                throw new Exception(@$"Could not create '{entityType.Name}' record.");
            }

            return rows;
        }

        public async Task<int> UpdateAsync(TModel model, Expression<Func<TEntity, bool>> predicate)
        {
            int rows = 0;
            Type entityType = typeof(TEntity);

            try
            {
                string values = string.Join(", ", entityType.GetProperties().Select(p => string.Format(@"{0} = @{0}", p.Name)));
                string query = string.Format(@"UPDATE {0} SET {1} {2}", GetTableName(entityType), values, GetFilter(predicate));

                TEntity entity = _mapper.Map<TEntity>(model);

                rows = await _connection.ExecuteAsync(query, entity, transaction: _transaction);
            }
            catch
            {
                throw new Exception(@$"Could not update '{entityType.Name}' record.");
            }

            return rows;
        }

        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            int rows = 0;
            Type entityType = typeof(TEntity);

            try
            {
                string query = string.Format(@"DELETE FROM {0} {1}", GetTableName(entityType), GetFilter(predicate));

                rows = await _connection.ExecuteAsync(query, transaction: _transaction);
            }
            catch
            {
                throw new Exception(@$"Could not delete '{entityType.Name}' record.");
            }

            return rows;
        }

        private string GetTableName(Type type)
        {
            TableAttribute table = type.GetCustomAttribute<TableAttribute>();

            if (table is null)
            {
                throw new Exception(@$"Could not get '{type}' table name.");
            }

            return table.Name;
        }

        private string GetFilter(Expression<Func<TEntity, bool>>? predicate = null)
        {
            string filter = string.Empty;

            if (predicate is not null)
            {
                SqlWhereClause swc = new SqlWhereClause();
                swc.Visit(predicate.Body);
                filter = swc.Get();
            }

            return filter;
        }
    }
}
