using StoreBytes.DataAccess.Models;

namespace StoreBytes.DataAccess.Data
{
    public interface IDatabaseData
    {
        void AddFileMetadata(int bucketId, string fileName, string fileHash, string filePath, long size, string contentType);
        void AddUserWithPassword(string email, string passwordHash);
        void CreateBucket(int userId, string bucketName);
        bool DeleteBucket(string hash);
        BucketModel GetBucketByHash(string hash);
        BucketModel? GetBucketById(int bucketId, int userId);
        BucketModel? GetBucketByName(string bucketName, int userId);
        List<FullBucketModel> GetBucketsByUserId(int userId);
        FileMetadataModel? GetFileMetadata(string bucketHash, string fileHash);
        List<FileModel> GetFilesByBucketHash(string bucketHash);
        UserModel GetUserByEmail(string email);
        UserTokenModel? GetUserTokenByApiKey(string apiKey);
        string SaveApiKey(int userId);
        bool SetBucketActiveState(string hash, bool isActive);
    }
}