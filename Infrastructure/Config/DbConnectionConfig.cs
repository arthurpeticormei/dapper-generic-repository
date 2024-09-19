using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace Infrastructure.Config
{
    public static class DbConnectionConfig
    {
        /// <summary>
        /// Registers database connections.
        /// </summary>
        /// <param name="services"></param>
        /// <returns><see cref="IServiceCollection"/> with registered database connections.</returns> 
        public static IServiceCollection ResolveDbConnection(this IServiceCollection services)
        {
            string connectionString = "connectionString";

            try
            {
                if (connectionString.IsNullOrEmpty())
                {
                    throw new Exception("Could not load connection string.");
                }

                services.AddScoped(sp => new SqlConnection(connectionString));
                services.AddScoped<IDbTransaction>(s =>
                {
                    SqlConnection connection = s.GetRequiredService<SqlConnection>();
                    connection.Open();
                    return connection.BeginTransaction();
                });
            }
            catch
            {
                throw;
            }

            return services;
        }
    }
}
