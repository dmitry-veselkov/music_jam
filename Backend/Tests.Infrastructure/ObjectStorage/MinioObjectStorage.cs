using Amazon.S3;
using Testcontainers.Minio;
using Xunit;

namespace Tests.Infrastructure.ObjectStorage;

public class MinioObjectStorage : IAsyncLifetime
{
    private readonly MinioContainer _container = new MinioBuilder(Constants.ObjectStorage.Image)
        .WithUsername(Constants.User.Username)
        .WithPassword(Constants.User.Password)
        .Build();

    public IAmazonS3 S3 { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var config = new AmazonS3Config
        {
            ServiceURL = _container.GetConnectionString(),
            ForcePathStyle = true,
            UseHttp = true
        };

        S3 = new AmazonS3Client(
            Constants.ObjectStorage.AccessKey,
            Constants.ObjectStorage.SecretKey,
            config);

        await S3.PutBucketAsync(Constants.ObjectStorage.BucketName);
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();
}
