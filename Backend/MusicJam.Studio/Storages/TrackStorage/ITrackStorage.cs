using MusicJam.Music;
using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Storages;

public interface ITrackStorage
{
    Task<Track?> GetTrackById(Guid id);

    Task<Track> SaveTrack(Track track);
}