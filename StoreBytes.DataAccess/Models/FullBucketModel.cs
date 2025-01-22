using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBytes.DataAccess.Models
{
    public class FullBucketModel
    {
        public int Id { get; set; }
        public string BucketName { get; set; }
        public string BucketHash { get; set; }
        public bool IsActive { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
    }
}
