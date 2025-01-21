
namespace StoreBytes.DataAccess.Models
{
    public class Bucket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HashedName { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
