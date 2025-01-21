using System.ComponentModel.DataAnnotations;

namespace StoreBytes.API.Models
{
    public class CreateBucketRequest
    {
        [Required]
        [MaxLength(255)]
        public string BucketName { get; set; }
    }
}
