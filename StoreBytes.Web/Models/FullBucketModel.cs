namespace StoreBytes.Web.Models
{
    public class FullBucketModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HashedName { get; set; }
        public bool IsActive { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
    }
}
