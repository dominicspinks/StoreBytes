using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using Npgsql;
using StoreBytes.Common.Configuration;

namespace StoreBytes.DataAccess.Databases
{
    public class PGSqlDataAccess : IPGSqlDataAccess
    {
        private readonly IConfiguration _config;
        private readonly string? _connectionString;

        static PGSqlDataAccess()
        {
            // Enable Dapper to map snake_case to PascalCase
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public PGSqlDataAccess(IConfiguration config)
        {
            _config = config;
            _connectionString = _config[ConfigurationKeys.Api.DatabaseUrl];
        }

        public List<T> LoadData<T, U>(
            string sqlStatement,
            U parameters,
            dynamic? options = null)
        {
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                List<T> rows = connection.Query<T>(sqlStatement, parameters, commandType: commandType).ToList();
                return rows;
            }
        }

        public void SaveData<T>(
            string sqlStatement,
            T parameters,
            dynamic? options = null)
        {
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sqlStatement, parameters, commandType: commandType);
            }
        }
    }
}
