using System.ComponentModel.DataAnnotations;

namespace StoreBytes.API.Models
{
    public class UpdateBucketRequestModel
    {
        [MaxLength(255)]
        public string? BucketName { get; set; }
        public bool? IsActive { get; set; }
    }
}
