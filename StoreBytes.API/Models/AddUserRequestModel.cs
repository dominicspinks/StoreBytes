using System.ComponentModel.DataAnnotations;

namespace StoreBytes.API.Models
{
    public class AddUserRequestModel
    {
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
    }
}
