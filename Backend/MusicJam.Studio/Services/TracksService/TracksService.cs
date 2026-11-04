using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MusicJam.Studio.Configurations;
using MusicJam.Studio.Models.Tracks;
using Shared;

namespace MusicJam.Studio.Services.TracksService;

internal class TracksService(IAmazonS3 s3, IOptions<ObjectStorageSettings> options) : ITracksService
{
    private readonly ObjectStorageSettings _settings = options.Value;

    public async Task<Result<Track, ITracksService.TrackError>> UploadTrack(
        IFormFile file,
        Guid? trackId = null,
        CancellationToken cancellationToken = default)
    {
        if (TryGetFileValidationError(file, out var validationError))
        {
            return validationError;
        }

        trackId ??= Guid.NewGuid();

        await using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = trackId.ToString(),
            InputStream = stream,
            AutoCloseStream = false
        };

        await s3.PutObjectAsync(request, cancellationToken);

        return new Track { Id = trackId.Value };
    }

    public async Task<Result<ITracksService.TrackError>> DeleteTrack(
        Guid trackId,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = trackId.ToString()
        };

        await s3.DeleteObjectAsync(request, cancellationToken);

        return Result<ITracksService.TrackError>.Success();
    }

    public async Task<Result<GetObjectResponse, ITracksService.TrackError>> GetTrack(
        Guid trackId,
        CancellationToken cancellationToken = default)
    {
        return await s3.GetObjectAsync(_settings.BucketName, trackId.ToString(), cancellationToken);
    }

    private bool TryGetFileValidationError(
        IFormFile file,
        [NotNullWhen(true)] out ITracksService.TrackError? validationError)
    {
        validationError = null;

        if (file.Length == 0)
        {
            validationError = ITracksService.TrackError.EmptyFile;
            return true;
        }

        if (file.Length > _settings.MaxFileSizeInBytes)
        {
            validationError = ITracksService.TrackError.FileTooLarge;
            return true;
        }

        return false;
    }
}
