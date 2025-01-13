using System.ComponentModel.DataAnnotations;

namespace StoreBytesAPI.Models
{
    public class AddUserRequest
    {
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
    }
}
