using System.ComponentModel.DataAnnotations;

namespace StoreBytes.API.Models
{
    public class CreateBucketRequestModel
    {
        [Required]
        [MaxLength(255)]
        public string BucketName { get; set; }
    }
}
