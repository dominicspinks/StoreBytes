namespace StoreBytes.Web.Models
{
    public class BucketModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BucketName { get; set; }
        public string BucketHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
