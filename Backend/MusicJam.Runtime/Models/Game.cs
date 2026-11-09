using MusicJam.Studio.Models.Packs;

namespace MusicJam.Runtime.Models;

public sealed class Game
{
    public Guid GameId { get; init; }
    public Guid PackId { get; set; }
    
    public Guid HostId { get; set; }
    
    public Pack Pack { get; set; }
}

