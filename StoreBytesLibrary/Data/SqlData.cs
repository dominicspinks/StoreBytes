using StoreBytesLibrary.Databases;
using StoreBytesLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBytesLibrary.Data
{
    public class SqlData : IDatabaseData
    {
        private readonly ISqlDataAccess _db;
        private const string _connectionStringName = "SqlConnection";

        public SqlData(ISqlDataAccess db)
        {
            _db = db;
        }

        public UserToken? GetUserTokenByApiKey(string apiKey)
        {
            const string sql = "SELECT t.Id, t.UserId, t.ApiKey, t.[Description], t.CreatedAt, t.ExpiresAt, t.IsActive FROM dbo.UserTokens t INNER JOIN dbo.Users u ON t.UserId = u.Id WHERE t.ApiKey = @ApiKey AND t.IsActive = 1 AND (t.ExpiresAt IS NULL OR t.ExpiresAt > GETDATE()) AND u.IsActive = 1";

            var results = _db.LoadData<UserToken, dynamic>(sql, new { ApiKey = apiKey }, _connectionStringName);
            return results.FirstOrDefault();
        }
    }
}
