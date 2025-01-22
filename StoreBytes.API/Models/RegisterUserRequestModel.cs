using System.ComponentModel.DataAnnotations;

namespace StoreBytes.API.Models
{
    public class RegisterUserRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
