using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace StoreBytes.DataAccess.Databases
{
    public class PGSqlDataAccess : IPGSqlDataAccess
    {
        private readonly IConfiguration _config;

        static PGSqlDataAccess()
        {
            // Enable Dapper to map snake_case to PascalCase
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public PGSqlDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public List<T> LoadData<T, U>(
            string sqlStatement,
            U parameters,
            dynamic? options = null)
        {
            string? connectionString = _config["DATABASE_URL"];
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
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
            string? connectionString = _config["DATABASE_URL"];
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(sqlStatement, parameters, commandType: commandType);
            }
        }
    }
}
