using Minio;
using Minio.DataModel.Args;

namespace TinyCity.Services
{
    public class S3Service
    {
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secretKey;

        public S3Service(string endpoint, string accessKey, string secretKey)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _accessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
        }

        private IMinioClient CreateClient()
        {
            return new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(_accessKey, _secretKey)
                .WithSSL()
                .Build();
        }

        public async Task UploadFileAsync(string bucketName, string objectKey, string filePath)
        {
            var client = CreateClient();

            bool found = await client.BucketExistsAsync(new BucketExistsArgs()
                .WithBucket(bucketName));

            if (!found)
            {
                await client.MakeBucketAsync(new MakeBucketArgs()
                    .WithBucket(bucketName));
            }

            await client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey)
                .WithFileName(filePath));
        }

        public async Task DownloadFileAsync(string bucketName, string objectKey, string destFilePath)
        {
            var client = CreateClient();

            await client.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey)
                .WithFile(destFilePath));
        }
    }
}
