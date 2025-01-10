using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace StoreBytesLibrary
{
    public class SqlDataAccess
    {
        private readonly IConfiguration config;

        public SqlDataAccess(IConfiguration config)
        {
            this.config = config;
        }

        public List<T> LoadData<T, U>(
            string sqlStatement,
            U parameters,
            string connectionStringName,
            dynamic? options = null)
        {
            string? connectionString = this.config.GetConnectionString(connectionStringName);
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(sqlStatement, parameters, commandType: commandType).ToList();
                return rows;
            }
        }

        public void SaveData<T>(
            string sqlStatement,
            T parameters,
            string connectionStringName,
            dynamic? options = null)
        {
            string? connectionString = this.config.GetConnectionString(connectionStringName);
            CommandType commandType = CommandType.Text;

            if (options?.IsStoredProcedure != null && options?.IsStoredProcedure == true)
            {
                commandType = CommandType.StoredProcedure;
            }

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(sqlStatement, parameters, commandType: commandType);
            }
        }
    }
}
