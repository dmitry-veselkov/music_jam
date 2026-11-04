using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MusicJam.Studio.Configurations;
using MusicJam.Studio.Models.Tracks;
using Shared;

namespace MusicJam.Studio.Services.TracksService;

internal class TracksService : ITracksService
{
    private readonly IAmazonS3 s3;
    private readonly ObjectStorageSettings settings;

    public TracksService(IAmazonS3 s3, IOptions<ObjectStorageSettings> options)
    {
        this.s3 = s3;
        settings = options.Value;
    }

    public async Task<Result<Track, ITracksService.TrackError>> UploadTrack(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
        {
            return ITracksService.TrackError.EmptyFile;
        }

        if (file.Length > settings.MaxFileSizeInBytes)
        {
            return ITracksService.TrackError.FileTooLarge;
        }

        var trackId = Guid.NewGuid();

        await using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = settings.BucketName,
            Key = trackId.ToString(),
            InputStream = stream,
            AutoCloseStream = false
        };

        await s3.PutObjectAsync(request, cancellationToken);

        return new Track { Id = trackId };
    }

    public async Task<Result<ITracksService.TrackError>> DeleteTrack(
        Guid trackId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<GetObjectResponse, ITracksService.TrackError>> GetTrack(
        Guid trackId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Track, ITracksService.TrackError>> UploadTrackInsteadOf(
        IFormFile file,
        Guid trackId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
