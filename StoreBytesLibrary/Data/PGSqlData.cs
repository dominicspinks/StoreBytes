﻿using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public PGSqlData(IPGSqlDataAccess db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public UserToken? GetUserTokenByApiKey(string apiKey)
        {
            // Hash the provided API key
            string hashedApiKey = SecurityHelper.HashBase64(apiKey);

            const string sql = @"
                    SELECT t.id, t.user_id, t.api_key, t.description, t.created_at, t.expires_at, t.is_active 
                    FROM user_tokens t 
                    INNER JOIN users u ON t.user_id = u.id 
                    WHERE t.api_key = @ApiKey 
                        AND t.is_active = true 
                        AND (t.expires_at IS NULL OR t.expires_at > NOW()) 
                        AND u.is_active = true";

            var results = _db.LoadData<UserToken, dynamic>(sql, new { ApiKey = apiKey });
            return results.FirstOrDefault();
        }

        public void SaveApiKey(int userId)
        {
            // Generate the API key
            string apiKey = SecurityHelper.GenerateApiKey();
            string hashedApiKey = SecurityHelper.HashBase64(apiKey);

            // Save the hashed API key to the database
            const string sql = @"
                    INSERT INTO user_tokens (user_id, api_key, created_at, is_active)
                    VALUES (@UserId, @ApiKey, NOW(), true)";

            _db.SaveData(sql, new { UserId = userId, ApiKey = hashedApiKey });

            // Output the generated API key (provide this to the user)
            Console.WriteLine("Generated API Key (provide this to the user): " + apiKey);
        }

        public void AddUser(string email)
        {
            const string sql = @"
                    INSERT INTO users (email, created_at, is_active)
                    VALUES (@Email, NOW(), true)";

            _db.SaveData(sql, new { Email = email });
        }

        public void CreateBucket(int userId, string bucketName)
        {
            string? secret = ConfigurationHelper.GetHashingSecret(_config);

            const string sqlCheck = @"
                    SELECT COUNT(1) 
                    FROM buckets 
                    WHERE user_id = @UserId AND name = @BucketName";

            int count = _db.LoadData<int, dynamic>(sqlCheck, new { UserId = userId, BucketName = bucketName }).FirstOrDefault();

            if (count > 0)
            {
                throw new Exception("Bucket name must be unique for the user.");
            }

            string hashedName = SecurityHelper.HashBase64Url($"{userId}{bucketName}", secret, 20);

            const string sqlInsert = @"
                    INSERT INTO buckets (name, user_id, hashed_name, created_at, is_active) 
                    VALUES (@BucketName, @UserId, @HashedName, NOW(), true)";

            _db.SaveData(sqlInsert, new { BucketName = bucketName, UserId = userId, HashedName = hashedName });
        }

        public void AddFileMetadata(int bucketId, string originalName, string hashedName, string filePath, long size, string contentType)
        {
            const string sql = @"
                    INSERT INTO files (bucket_id, original_name, hashed_name, file_path, size, content_type, created_at)
                    VALUES (@BucketId, @OriginalName, @HashedName, @FilePath, @Size, @ContentType, NOW())";

            _db.SaveData(sql, new
            {
                BucketId = bucketId,
                OriginalName = originalName,
                HashedName = hashedName,
                FilePath = filePath,
                Size = size,
                ContentType = contentType
            });
        }

        public Bucket? GetBucketById(int bucketId, int userId)
        {
            const string sql = @"
                    SELECT id, name, hashed_name, user_id, created_at, is_active
                    FROM buckets
                    WHERE id = @BucketId AND user_id = @UserId AND is_active = true";

            var results = _db.LoadData<Bucket, dynamic>(sql, new { BucketId = bucketId, UserId = userId });

            return results.FirstOrDefault();
        }

        public FileMetadata? GetFileMetadata(string bucketHash, string fileHash)
        {
            const string sql = @"
                    SELECT f.original_name, f.content_type, f.file_path
                    FROM files f
                    INNER JOIN buckets b ON f.bucket_id = b.id
                    WHERE b.hashed_name = @BucketHash AND f.hashed_name = @FileHash";

            return _db.LoadData<FileMetadata, dynamic>(
                sql,
                new { BucketHash = bucketHash, FileHash = fileHash }
            ).FirstOrDefault();
        }
    }
}
