using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrastructure.Data;
using DbTrack = MusicJam.Studio.Infrastructure.Entities.Track;
using Track = MusicJam.Studio.Models.Tracks.Track;

namespace MusicJam.Studio.Storages.TrackStorage;

public class TrackStorage(AppDbContext db) : ITrackStorage
{
    private static Track ToModel(DbTrack e) => new() { Id = e.Id };

    public static DbTrack ToDbModel(Track e) => new() { ObjectKey = e.Id };

    public async Task<Track?> GetTrackById(Guid id)
    {
        var track = await db.Tracks
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        return track is null ? null : ToModel(track);
    }

    public async Task<Track> SaveTrack(Track track)
    {
        await db.Tracks.AddAsync(new DbTrack { ObjectKey = track.Id });

        return track;
    }
}
