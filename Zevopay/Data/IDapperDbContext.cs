using System.Data;

namespace Zevopay.Data
{
        public interface IDapperDbContext
        {
        IDbConnection ConnectDb { get; }
            Task<IEnumerable<T>> QueryAsync<T>(string text, object? parameters = default, int? timeout = null, CommandType? type = null);
            Task<int> ExecuteAsync(string text, object? parameters = default, int? timeout = null, CommandType? type = null);

            Task<T> QueryFirstOrDefaultAsync<T>(string text, object? parameters = default, int? timeout = null, CommandType? type = null);
        }

    

}
