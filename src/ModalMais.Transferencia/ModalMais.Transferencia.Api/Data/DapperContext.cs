using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace ModalMais.Transferencia.Api.Data
{
    public class DapperContext : IDisposable
    {
        private readonly IDbConnection _connection;

        public DapperContext(IDbConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public async Task<IEnumerable<T>> Run<T>(string query, object data)
        {
            var result = await _connection.QueryAsync<T>(query, data);
            return result;
        }

        public async Task<T> Single<T>(string query, object data)
        {
            var result = await _connection.QueryFirstOrDefaultAsync<T>(query, data);
            return result;
        }
    }
}