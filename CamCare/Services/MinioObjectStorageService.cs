using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using CamCare.Interfaces.Services;
using CamCare.Options;
using Microsoft.Extensions.Options;

namespace CamCare.Services
{
    public class MinioObjectStorageService : IObjectStorageService
    {
        private readonly ObjectStorageOptions _options;
        private readonly ILogger<MinioObjectStorageService> _logger;
        private readonly IAmazonS3 _s3Client;
        private readonly SemaphoreSlim _bucketSemaphore = new(1, 1);
        private volatile bool _bucketInitialized;

        public MinioObjectStorageService(IOptions<ObjectStorageOptions> options, ILogger<MinioObjectStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_options.Endpoint))
                throw new InvalidOperationException("ObjectStorage:Endpoint ist nicht konfiguriert.");

            if (string.IsNullOrWhiteSpace(_options.AccessKey) || string.IsNullOrWhiteSpace(_options.SecretKey))
                throw new InvalidOperationException("ObjectStorage AccessKey/SecretKey sind nicht konfiguriert.");

            var config = new AmazonS3Config
            {
                ServiceURL = _options.Endpoint,
                ForcePathStyle = true,
                UseHttp = !_options.UseSsl
            };

            _s3Client = new AmazonS3Client(new BasicAWSCredentials(_options.AccessKey, _options.SecretKey), config);
        }

        public async Task UploadAsync(
            string objectKey,
            Stream content,
            string contentType,
            IReadOnlyDictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            await EnsureBucketExistsAsync(cancellationToken);

            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                InputStream = content,
                AutoCloseStream = false,
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType
            };

            if (metadata is not null)
            {
                foreach (var item in metadata)
                {
                    if (!string.IsNullOrWhiteSpace(item.Key) && item.Value is not null)
                    {
                        request.Metadata[item.Key] = ToSafeHeaderValue(item.Value);
                    }
                }
            }

            await _s3Client.PutObjectAsync(request, cancellationToken);
        }

        private static string ToSafeHeaderValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            foreach (var ch in value)
            {
                if (ch > 127 || ch == '\r' || ch == '\n')
                {
                    // S3 metadata is transmitted as HTTP headers and must be ASCII-safe.
                    return Uri.EscapeDataString(value);
                }
            }

            return value;
        }

        public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                return;

            await EnsureBucketExistsAsync(cancellationToken);

            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("ObjectKey darf nicht leer sein.", nameof(objectKey));

            await EnsureBucketExistsAsync(cancellationToken);

            var response = await _s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey
            }, cancellationToken);

            // Copy to memory so response can be disposed safely before returning.
            var memory = new MemoryStream();
            using (response)
            {
                await response.ResponseStream.CopyToAsync(memory, cancellationToken);
            }

            memory.Position = 0;
            return memory;
        }

        public async Task<string> GetReadUrlAsync(string objectKey, TimeSpan expiresIn)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                return string.Empty;

            await EnsureBucketExistsAsync();

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.Add(expiresIn),
                Verb = HttpVerb.GET
            };

            return _s3Client.GetPreSignedURL(request);
        }

        private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken = default)
        {
            if (_bucketInitialized)
                return;

            await _bucketSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (_bucketInitialized)
                    return;

                var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _options.BucketName);
                if (!exists)
                {
                    _logger.LogInformation("Erstelle ObjectStorage Bucket {BucketName}", _options.BucketName);
                    await _s3Client.PutBucketAsync(new PutBucketRequest
                    {
                        BucketName = _options.BucketName
                    }, cancellationToken);
                }

                _bucketInitialized = true;
            }
            finally
            {
                _bucketSemaphore.Release();
            }
        }
    }
}
