using StoreBytesLibrary.Databases;
using StoreBytesLibrary.Models;
using StoreBytesLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBytesLibrary.Data
{
    public class PGSqlData : IDatabaseData
    {
        private readonly IPGSqlDataAccess _db;
        private const string _connectionStringName = "PostgresConnection";

        public PGSqlData(IPGSqlDataAccess db)
        {
            _db = db;
        }

        public UserToken? GetUserTokenByApiKey(string apiKey)
        {
            // Hash the provided API key
            string hashedApiKey = SecurityHelper.HashApiKey(apiKey);

            const string sql = @"
                    SELECT t.id, t.user_id, t.api_key, t.description, t.created_at, t.expires_at, t.is_active 
                    FROM user_tokens t 
                    INNER JOIN users u ON t.user_id = u.id 
                    WHERE t.api_key = @ApiKey 
                        AND t.is_active = true 
                        AND (t.expires_at IS NULL OR t.expires_at > NOW()) 
                        AND u.is_active = true";

            var results = _db.LoadData<UserToken, dynamic>(sql, new { ApiKey = apiKey }, _connectionStringName);
            return results.FirstOrDefault();
        }

        public void SaveApiKey(int userId)
        {
            // Generate the API key
            string apiKey = SecurityHelper.GenerateApiKey();
            string hashedApiKey = SecurityHelper.HashApiKey(apiKey);

            // Save the hashed API key to the database
            const string sql = @"
                    INSERT INTO user_tokens (user_id, api_key, created_at, is_active)
                    VALUES (@UserId, @ApiKey, NOW(), true)";

            _db.SaveData(sql, new { UserId = userId, ApiKey = hashedApiKey }, "PostgresConnection");

            // Output the generated API key (provide this to the user)
            Console.WriteLine("Generated API Key (provide this to the user): " + apiKey);
        }
    }
}
