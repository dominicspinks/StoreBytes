
namespace StoreBytes.DataAccess.Models
{
    public class UserTokenModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ApiKey { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
