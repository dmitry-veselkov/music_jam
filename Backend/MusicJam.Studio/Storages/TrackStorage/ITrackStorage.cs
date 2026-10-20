using MusicJam.Music;
using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Storages.TrackStorage;

public interface ITrackStorage
{
    Task<Track?> TryGetTrackById(Guid id);

    Task<Track> SaveTrack(Track track);
}