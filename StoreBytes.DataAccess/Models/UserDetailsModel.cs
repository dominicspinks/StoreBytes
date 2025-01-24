
namespace StoreBytes.DataAccess.Models
{
    public class UserDetailsModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
