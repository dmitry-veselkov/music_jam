using MusicJam.Studio.Models.Tracks;

namespace MusicJam.Studio.Storages.TrackStorage;

public interface ITrackStorage
{
    Task<Track?> GetTrackById(Guid id);

    Task<Track> SaveTrack(Track track);
}
