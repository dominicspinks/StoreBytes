using StoreBytes.DataAccess.Interfaces;

namespace StoreBytes.DataAccess.Models
{
    public class FileModel : IOwnable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BucketId { get; set; }
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
