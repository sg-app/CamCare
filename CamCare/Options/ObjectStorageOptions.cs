namespace CamCare.Options
{
    public class ObjectStorageOptions
    {
        public const string SectionName = "ObjectStorage";

        public string Endpoint { get; set; } = "http://localhost:9000";
        public string AccessKey { get; set; } = "minioadmin";
        public string SecretKey { get; set; } = "minioadmin";
        public string BucketName { get; set; } = "camcare-files";
        public bool UseSsl { get; set; }
        public int PresignedUrlLifetimeMinutes { get; set; } = 15;
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
    }
}
