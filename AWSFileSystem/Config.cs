namespace AWSFileSystem
{
    public class Config
    {
        public string? awsAccessKey { get; set; }
        public string? awsSecretKey { get; set; }
        public string? bucketName { get; set; }
        public string? region { get; set; }
        public long smallStreamSize { get; set; } = 5 * 1024 * 1024;
    }
}
