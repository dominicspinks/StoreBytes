namespace StoreBytes.Web.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BucketId { get; set; }
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public string FilePath { get; set; }
        public string? Url { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
