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

    Task<Result<Track, TrackError>> UploadTrack(IFormFile file, CancellationToken cancellationToken = default);

    Task<Result<TrackError>> DeleteTrack(Guid trackId, CancellationToken cancellationToken = default);

    Task<Result<GetObjectResponse, TrackError>> GetTrack(Guid trackId, CancellationToken cancellationToken = default);

    Task<Result<Track, TrackError>> UploadTrackInsteadOf(IFormFile file, Guid trackId,
        CancellationToken cancellationToken = default);
}
