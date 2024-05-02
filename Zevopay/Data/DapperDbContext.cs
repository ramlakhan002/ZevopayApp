namespace Zevopay.Data
{
    using Dapper;
    using Microsoft.Data.SqlClient;
    using System.Data;

    namespace PrimeBuilderAdmin.Context
    {
        public class DapperDbContext : IDapperDbContext
        {
            private readonly int CommandTimeout = 300;
            private readonly string ConnectionString = string.Empty;

            public DapperDbContext(IConfiguration configuration)
            {
                ConnectionString = configuration.GetConnectionString("ZevopayDb") ?? throw new ArgumentNullException("Connection string");
            }

            public IDbConnection ConnectDb => new SqlConnection(ConnectionString);

            public async Task<IEnumerable<T>> QueryAsync<T>(string text, object? parameters = default, int? timeout = null, CommandType? type = null)
            {
                using var connection = new SqlConnection(ConnectionString);
                var command = new CommandDefinition(text, parameters, commandTimeout: (timeout == null ? CommandTimeout : timeout), commandType: type ?? CommandType.Text);
                return await connection.QueryAsync<T>(command);
            }

            public async Task<int> ExecuteAsync(string text, object? parameters = default, int? timeout = null, CommandType? type = null)
            {
                using var connection = new SqlConnection(ConnectionString);
                var command = new CommandDefinition(text, parameters, commandTimeout: (timeout == null ? CommandTimeout : timeout), commandType: type ?? CommandType.Text);
                return await connection.ExecuteAsync(command);
            }

            public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object? parameters = default, int? timeout = null, CommandType? type = null)
            {
                using var connection = new SqlConnection(ConnectionString);
                var command = new CommandDefinition(text, parameters, commandTimeout: (timeout == null ? CommandTimeout : timeout), commandType: type ?? CommandType.Text);
                return await connection.QueryFirstOrDefaultAsync<T>(command);
            }

        }
    }

}
