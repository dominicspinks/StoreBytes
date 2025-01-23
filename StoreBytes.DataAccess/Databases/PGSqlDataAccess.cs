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

        public IDbTransaction BeginTransaction()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection.BeginTransaction();
        }

        public List<T> LoadData<T, U>(
            string sqlStatement,
            U parameters,
            dynamic? options = null,
            IDbTransaction? transaction = null)
        {
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            using (IDbConnection connection = transaction?.Connection ?? new NpgsqlConnection(_connectionString))
            {
                return connection.Query<T>(sqlStatement, parameters, commandType: commandType, transaction: transaction).ToList();
            }
        }

        public int SaveData<T>(
            string sqlStatement,
            T parameters,
            dynamic? options = null,
            IDbTransaction? transaction = null)
        {
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            if (transaction != null && transaction.Connection != null)
            {
                // Use the transaction's connection
                return transaction.Connection.Execute(sqlStatement, parameters, transaction: transaction, commandType: commandType);
            }

            // Fallback to a new connection if no transaction is provided
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.Execute(sqlStatement, parameters, commandType: commandType);
            }
        }

        public T ExecuteTransaction<T>(Func<IDbTransaction, T> transactionalOperation)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Pass the transaction to the provided operation
                        T result = transactionalOperation(transaction);

                        // Commit the transaction
                        transaction.Commit();

                        return result;
                    }
                    catch
                    {
                        // Rollback in case of error
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        
        }
    }
}
