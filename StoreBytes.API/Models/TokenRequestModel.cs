using System.ComponentModel.DataAnnotations;

namespace StoreBytes.API.Models
{
    public class TokenRequestModel
    {
        [Required]
        public string ApiKey { get; set; }
    }
}
