using StoreBytesLibrary.Models;

namespace StoreBytesLibrary.Data
{
    public interface IDatabaseData
    {
        void AddFileMetadata(int bucketId, string originalName, string hashedName, string filePath, long size, string contentType);
        void AddUser(string email);
        void CreateBucket(int userId, string bucketName);
        Bucket? GetBucketById(int bucketId, int userId);
        FileMetadata? GetFileMetadata(string bucketHash, string fileHash);
        UserToken? GetUserTokenByApiKey(string apiKey);
        string SaveApiKey(int userId);
    }
}