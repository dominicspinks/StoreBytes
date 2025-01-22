
using StoreBytes.DataAccess.Interfaces;

namespace StoreBytes.DataAccess.Models
{
    public class BucketModel : IOwnable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BucketName { get; set; }
        public string BucketHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
