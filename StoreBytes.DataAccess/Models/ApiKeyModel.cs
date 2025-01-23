
namespace StoreBytes.DataAccess.Models
{
    public class ApiKeyModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string KeyHash { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
