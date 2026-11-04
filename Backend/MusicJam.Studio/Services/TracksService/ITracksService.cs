using Amazon.S3.Model;
using MusicJam.Studio.Models.Tracks;
using Shared;

namespace MusicJam.Studio.Services.TracksService;

public interface ITracksService
{
    public enum TrackError
    {
        EmptyFile,
        FileTooLarge,
        TrackNotFound,
    }

    Task<Result<Track, TrackError>> UploadTrack(
        IFormFile file,
        Guid? trackId = null, // если не null - хотим перезаписать существующий файл
        CancellationToken cancellationToken = default);

    Task<Result<TrackError>> DeleteTrack(Guid trackId, CancellationToken cancellationToken = default);

    Task<Result<long, TrackError>> GetTrackSize(Guid trackId, CancellationToken cancellationToken = default);

    Task<Result<GetObjectResponse, TrackError>> GetTrack(Guid trackId, CancellationToken cancellationToken = default);

    Task<Result<GetObjectResponse, TrackError>> GetTrackRange(
        Guid trackId,
        long start,
        long end,
        CancellationToken cancellationToken = default);
}
