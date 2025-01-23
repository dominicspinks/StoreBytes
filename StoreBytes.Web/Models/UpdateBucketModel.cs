using System.ComponentModel.DataAnnotations;

namespace StoreBytes.Web.Models
{
    public class UpdateBucketModel
    {
        [MaxLength(255)]
        public string? BucketName { get; set; }
        public bool? IsActive { get; set; }
    }
}
