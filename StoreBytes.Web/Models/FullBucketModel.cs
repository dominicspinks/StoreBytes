﻿namespace StoreBytes.Web.Models
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
