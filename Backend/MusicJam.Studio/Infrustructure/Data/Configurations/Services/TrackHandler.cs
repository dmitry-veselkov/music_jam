using MusicJam.Studio.Infrustructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations.Services;

public class TrackHandler
{
    private readonly AppDbContext _dataContext;
    
    public TrackHandler(AppDbContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<Guid> Insert(Track track)
    {
        _dataContext.Tracks.Add(track);
        await _dataContext.SaveChangesAsync();
        return track.Id;
    }
    
    public async Task Delete(Track track)
    {
        _dataContext.Tracks.Remove(track);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task Delete(Guid id)
    {
        var trackDb = _dataContext.Tracks
            .FirstOrDefault(x => x.Id == id);
        if (trackDb is null)
            return;
        _dataContext.Tracks.Remove(
            trackDb);
        await _dataContext.SaveChangesAsync();
    }
}