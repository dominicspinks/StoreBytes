using Microsoft.Extensions.Configuration;
using StoreBytes.DataAccess.Databases;
using StoreBytes.DataAccess.Models;
using StoreBytes.Common.Utilities;
using StoreBytes.Common.Configuration;

namespace StoreBytes.DataAccess.Data
{
    public class PGSqlData : IDatabaseData
    {
        private readonly IPGSqlDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _hashSecret = "";

        public PGSqlData(IPGSqlDataAccess db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _hashSecret = _config[ConfigurationKeys.Shared.HashSecret] ?? "";
        }

        public UserTokenModel? GetUserTokenByApiKey(string apiKey)
        {
            // Hash the provided API key
            string hashedApiKey = SecurityHelper.HashBase64(apiKey, _hashSecret);

            const string sql = @"
                    SELECT t.id, t.user_id, t.api_key, t.description, t.created_at, t.expires_at, t.is_active 
                    FROM user_tokens t 
                    INNER JOIN users u ON t.user_id = u.id 
                    WHERE t.api_key = @ApiKey 
                        AND t.is_active = true 
                        AND (t.expires_at IS NULL OR t.expires_at > NOW()) 
                        AND u.is_active = true";

            var results = _db.LoadData<UserTokenModel, dynamic>(sql, new { ApiKey = hashedApiKey });
            return results.FirstOrDefault();
        }

        public string SaveApiKey(int userId)
        {
            // Generate the API key
            string apiKey = SecurityHelper.GenerateApiKey();
            string hashedApiKey = SecurityHelper.HashBase64(apiKey, _hashSecret);

            // Save the hashed API key to the database
            const string sql = @"
                    INSERT INTO user_tokens (user_id, api_key, created_at, is_active)
                    VALUES (@UserId, @ApiKey, NOW(), true)";

            try
            {
                _db.SaveData(sql, new { UserId = userId, ApiKey = hashedApiKey });

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create API key");
            }

            // Output the generated API key (provide this to the user)
            Console.WriteLine("Generated API Key (provide this to the user): " + apiKey);
            return apiKey;
        }

        public void CreateBucket(int userId, string bucketName)
        {
            const string sqlCheck = @"
                    SELECT COUNT(1) 
                    FROM buckets 
                    WHERE user_id = @UserId AND bucket_name = @BucketName";

            int count = _db.LoadData<int, dynamic>(sqlCheck, new { UserId = userId, BucketName = bucketName }).FirstOrDefault();

            if (count > 0)
            {
                throw new Exception("Bucket name must be unique for the user.");
            }

            string bucketHash = SecurityHelper.HashBase64Url($"{Guid.NewGuid()}", _hashSecret, 20);

            const string sqlInsert = @"
                    INSERT INTO buckets (bucket_name, user_id, bucket_hash, created_at, is_active) 
                    VALUES (@BucketName, @UserId, @BucketHash, NOW(), true)";

            _db.SaveData(sqlInsert, new { BucketName = bucketName, UserId = userId, BucketHash = bucketHash });
        }

        public void AddFileMetadata(int bucketId, string fileName, string fileHash, string filePath, long size, string contentType)
        {
            const string sql = @"
                    INSERT INTO files (bucket_id, file_name, file_hash, file_path, size, content_type, created_at)
                    VALUES (@BucketId, @FileName, @FileHash, @FilePath, @Size, @ContentType, NOW())";

            _db.SaveData(sql, new
            {
                BucketId = bucketId,
                FileName = fileName,
                FileHash = fileHash,
                FilePath = filePath,
                Size = size,
                ContentType = contentType
            });
        }

        public BucketModel? GetBucketById(int bucketId, int userId)
        {
            const string sql = @"
                    SELECT id, bucket_name, bucket_hash, user_id, created_at, is_active
                    FROM buckets
                    WHERE id = @BucketId AND user_id = @UserId AND is_active = true";

            var results = _db.LoadData<BucketModel, dynamic>(sql, new { BucketId = bucketId, UserId = userId });

            return results.FirstOrDefault();
        }

        public BucketModel? GetBucketByName(string bucketName, int userId)
        {
            const string sql = @"
                    SELECT id, bucket_name, bucket_hash, user_id, created_at, is_active
                    FROM buckets
                    WHERE bucket_name = @BucketName AND user_id = @UserId AND is_active = true";

            var results = _db.LoadData<BucketModel, dynamic>(sql, new { BucketName = bucketName, UserId = userId });

            return results.FirstOrDefault();
        }

        public FileMetadataModel? GetFileMetadata(string bucketHash, string fileHash)
        {
            const string sql = @"
                    SELECT f.file_name, f.content_type, f.file_path
                    FROM files f
                    INNER JOIN buckets b ON f.bucket_id = b.id
                    WHERE b.bucket_hash = @BucketHash AND f.file_hash = @FileHash";

            return _db.LoadData<FileMetadataModel, dynamic>(
                sql,
                new { BucketHash = bucketHash, FileHash = fileHash }
            ).FirstOrDefault();
        }

        public void AddUserWithPassword(string email, string passwordHash)
        {
            string sql = "INSERT INTO users (email, password_hash) VALUES (@Email, @PasswordHash)";
            _db.SaveData(sql, new { Email = email, PasswordHash = passwordHash });
        }

        public UserModel GetUserByEmail(string email)
        {
            string sql = "SELECT id, email,created_at, is_active, password_hash FROM users WHERE email = @Email AND is_active = true";
            return _db.LoadData<UserModel, dynamic>(sql, new { Email = email }).FirstOrDefault();
        }

        public List<FullBucketModel> GetBucketsByUserId(int userId)
        {
            string sql = @"
                    SELECT 
                        b.id,
                        b.bucket_name,
                        b.bucket_hash,
                        b.is_active,
                        COUNT(f.id) AS FileCount,
                        COALESCE(SUM(f.size), 0) AS TotalSize
                    FROM buckets b
                    LEFT JOIN files f ON b.id = f.bucket_id
                    WHERE b.user_id = @UserId
                    GROUP BY b.id, b.bucket_name, b.bucket_hash, b.is_active;
                ";

            return _db.LoadData<FullBucketModel, dynamic>(sql, new { UserId = userId });
        }

        public BucketModel GetBucketByHash(string hash)
        {
            string sql = @"
                SELECT 
                    id, 
                    user_id, 
                    bucket_name, 
                    bucket_hash, 
                    is_active, 
                    created_at 
                FROM buckets 
                WHERE bucket_hash = @Hash 
                ORDER BY created_at
                LIMIT 1";
            return _db.LoadData<BucketModel, dynamic>(sql, new { Hash = hash }).FirstOrDefault();
        }

        public List<FileModel> GetFilesByBucketHash(string bucketHash)
        {
            string sql = @"
                SELECT 
                    f.id, 
                    b.user_id,
                    f.bucket_id, 
                    f.file_name, 
                    f.file_hash,
                    f.file_path, 
                    f.size, 
                    f.created_at 
                FROM files f
                INNER JOIN buckets b ON f.bucket_id = b.id
                WHERE b.bucket_hash = @Hash
                ORDER BY f.created_at";
            return _db.LoadData<FileModel, dynamic>(sql, new { Hash = bucketHash });
        }

        public bool SetBucketActiveState(string hash, bool isActive)
        {
            string sql = "UPDATE buckets SET is_active = @IsActive WHERE bucket_hash = @Hash";
            int rowsAffected = _db.SaveData(sql, new { IsActive = isActive, Hash = hash });
            return rowsAffected > 0;
        }

        public bool DeleteBucket(string hash)
        {
            return _db.ExecuteTransaction(transaction =>
            {
                // Delete associated files
                string deleteFilesSql = @"
                    DELETE FROM files 
                    WHERE bucket_id = (
                        SELECT id FROM buckets WHERE bucket_hash = @Hash
                    )";
                _db.SaveData(deleteFilesSql, new { Hash = hash }, transaction: transaction);

                // Delete the bucket
                string deleteBucketSql = "DELETE FROM buckets WHERE bucket_hash = @Hash";
                int rowsAffected = _db.SaveData(deleteBucketSql, new { Hash = hash }, transaction: transaction);

                return rowsAffected > 0;
            });
        }

        public bool UpdateBucketDetails(string bucketHash, string bucketName, bool isActive)
        { 
            string sql = $@"
                UPDATE buckets
                SET 
                    bucket_name = @BucketName, 
                    is_active = @IsActive
                WHERE bucket_hash = @BucketHash";

            int rowsAffected = _db.SaveData(sql, new
            {
                BucketHash = bucketHash,
                BucketName = bucketName,
                IsActive = isActive
            });

            return rowsAffected > 0;
        }
    }
}
