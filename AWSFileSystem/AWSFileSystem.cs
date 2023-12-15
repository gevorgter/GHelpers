using Amazon;
using Amazon.S3.Model;
using GHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AWSFileSystem
{
    public class AWSFileSystem
    {
        readonly Amazon.S3.AmazonS3Client _s3Client;
        bool _isBucketExists = false;
        readonly RegionEndpoint _region;
        readonly string _bucketName;
        readonly long _smallStreamSize;
        readonly ILogger<AWSFileSystem> _logger;

        public AWSFileSystem(IConfiguration config, ILogger<AWSFileSystem> logger)
        {
            _logger = logger;
            var cfg = config.GetSection("aws").Get<Config>();
            cfg.ThrowExceptionIfNull("You must have \"aws\" section in config");
            cfg.awsAccessKey.ThrowExceptionIfNull("You must specify awsAccessKey in aws config section");
            cfg.awsSecretKey.ThrowExceptionIfNull("You must specify awsSecretKey in aws config section");
            cfg.bucketName.ThrowExceptionIfNull("You must specify bucketName in aws config section");
            cfg.region.ThrowExceptionIfNull("You must specify region in aws config section");

            _region = RegionEndpoint.GetBySystemName(cfg.region);
            _smallStreamSize = cfg.smallStreamSize;
            var credential = new Amazon.Runtime.BasicAWSCredentials(cfg.awsAccessKey, cfg.awsSecretKey);
            _bucketName = cfg.bucketName;
            _s3Client = new Amazon.S3.AmazonS3Client(credential, _region);
        }

        public async ValueTask MakeSureBucketExists(CancellationToken cancellationToken = default)
        {
            var rq = new PutBucketRequest
            {
                BucketName = _bucketName,
                UseClientRegion = true,
            };
            await _s3Client.PutBucketAsync(rq, cancellationToken);
            _isBucketExists = true;
        }

        public async ValueTask DeleteBucket()
        {
            await _s3Client.DeleteBucketAsync(new DeleteBucketRequest
            {
                BucketName = _bucketName,
                UseClientRegion = true,
            });
        }

        public string Separator => "/";

        public async ValueTask DeleteObject(string key)
        {
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                Key = key
            });
        }

        public async ValueTask UploadSmallStream(string key, Stream st, CancellationToken cancellationToken = default)
        {
            await _s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = st,
            }, cancellationToken);
        }

        public async ValueTask UploadStream(string key, long size, Stream st, CancellationToken cancellationToken = default)
        {
            if (_smallStreamSize >= size)
                await UploadSmallStream(key, st, cancellationToken);
            else
            {
                string? uploadId = null;
                try
                {
                    var rp = await _s3Client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest()
                    {
                        BucketName = _bucketName,
                        Key = key,
                    }, cancellationToken);
                    uploadId = rp.UploadId;
                    List<PartETag> parts = new List<PartETag>();
                    for (int partNumber = 1; size > 0; partNumber++)
                    {
                        long partSize = Math.Min(size, _smallStreamSize);
                        var rp1 = await _s3Client.UploadPartAsync(new UploadPartRequest
                        {
                            Key = key,
                            BucketName = _bucketName,
                            UploadId = uploadId,
                            InputStream = st,
                            PartNumber = partNumber,
                            PartSize = partSize,
                            IsLastPart = partSize == size,
                        }, cancellationToken);
                        parts.Add(new PartETag(partNumber, rp1.ETag));
                        size -= partSize;
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    await _s3Client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        UploadId = uploadId,
                        PartETags = parts,
                    }, cancellationToken);
                }
                catch (Exception ex)
                {
                    if (!(ex is OperationCanceledException))
                        _logger.LogError(ex, "S3 threw an exception");

                    if (uploadId != null)
                    {
                        await _s3Client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
                        {
                            BucketName = _bucketName,
                            Key = key,
                            UploadId = uploadId,
                        }); //ignore cancelation token here, we need to cancel multipart upload even if we requested cancelation
                    }
                    throw;
                }
            }
        }

        public async ValueTask GetObject(string key, Stream st, CancellationToken cancellationToken = default)
        {
            using var rp = await _s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
            }, cancellationToken);
            await rp.ResponseStream.CopyToAsync(st, cancellationToken);
        }
    }
}
