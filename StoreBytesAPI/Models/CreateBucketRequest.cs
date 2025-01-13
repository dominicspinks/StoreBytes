using System.ComponentModel.DataAnnotations;

namespace StoreBytesAPI.Models
{
    public class CreateBucketRequest
    {
        [Required]
        [MaxLength(255)]
        public string BucketName { get; set; }
    }
}
