using Amazon.S3;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace Tests.Infrastructure.ObjectStorage;

public class S3MockObjectStorage : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder("adobe/s3mock:latest")
        .WithPortBinding(Constants.ObjectStorage.Port, true)
        .WithWaitStrategy(
            Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(Constants.ObjectStorage.Port)
                    .ForPath("/favicon.ico")))
        .Build();

    public IAmazonS3 S3 { get; private set; } = null!;

    public string ConnectionString;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        ConnectionString =
            $"http://{_container.Hostname}:{_container.GetMappedPublicPort(Constants.ObjectStorage.Port)}";

        var config = new AmazonS3Config
        {
            ServiceURL = ConnectionString,
            ForcePathStyle = true,
            UseHttp = true,
            AuthenticationRegion = "us-east-1"
        };

        S3 = new AmazonS3Client(
            Constants.ObjectStorage.AccessKey,
            Constants.ObjectStorage.SecretKey,
            config);

        await S3.PutBucketAsync(Constants.ObjectStorage.BucketName);
    }

    public async Task DisposeAsync()
    {
        S3.Dispose();
        await _container.DisposeAsync();
    }
}
