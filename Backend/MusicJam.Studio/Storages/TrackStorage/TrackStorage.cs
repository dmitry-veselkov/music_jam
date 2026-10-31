using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbTrack = MusicJam.Studio.Infrustructure.Entities.Track;
using Track = MusicJam.Music.Track;
namespace MusicJam.Studio.Storages;

public class TrackStorage(AppDbContext db) : ITrackStorage
{
    private static Track ToModel(DbTrack e) => new()
    {
        Link = e.Link,
    };
    
    public static DbTrack ToDbModel(Track e) => new()
    {
        Link = e.Link,
    };


    public async Task<Track?> GetTrackById(Guid id)
    {
        var track = await db.Tracks
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        return track is null ? null : ToModel(track);
    }

    public async Task<Track> SaveTrack(Track track)
    {
        await db.Tracks.AddAsync(new DbTrack()
        {
            Link = track.Link
        });
        return track;
    }
}
