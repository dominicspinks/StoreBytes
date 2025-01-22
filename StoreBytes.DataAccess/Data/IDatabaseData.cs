using StoreBytes.DataAccess.Models;

namespace StoreBytes.DataAccess.Data
{
    public interface IDatabaseData
    {
        void AddFileMetadata(int bucketId, string originalName, string hashedName, string filePath, long size, string contentType);
        void AddUserWithPassword(string email, string passwordHash);
        void CreateBucket(int userId, string bucketName);
        BucketModel? GetBucketById(int bucketId, int userId);
        BucketModel? GetBucketByName(string bucketName, int userId);
        List<FullBucketModel> GetBucketsByUserId(int userId);
        FileMetadataModel? GetFileMetadata(string bucketHash, string fileHash);
        UserModel GetUserByEmail(string email);
        UserTokenModel? GetUserTokenByApiKey(string apiKey);
        string SaveApiKey(int userId);
    }
}