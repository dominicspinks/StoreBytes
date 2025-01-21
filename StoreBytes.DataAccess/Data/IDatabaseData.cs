using StoreBytes.DataAccess.Models;

namespace StoreBytes.DataAccess.Data
{
    public interface IDatabaseData
    {
        void AddFileMetadata(int bucketId, string originalName, string hashedName, string filePath, long size, string contentType);
        void AddUserWithPassword(string email, string passwordHash);
        void CreateBucket(int userId, string bucketName);
        Bucket? GetBucketById(int bucketId, int userId);
        Bucket? GetBucketByName(string bucketName, int userId);
        FileMetadata? GetFileMetadata(string bucketHash, string fileHash);
        UserModel GetUserByEmail(string email);
        UserToken? GetUserTokenByApiKey(string apiKey);
        string SaveApiKey(int userId);
    }
}