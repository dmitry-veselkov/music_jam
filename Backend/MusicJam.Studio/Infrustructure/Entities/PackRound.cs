using MusicJam.Domain.Common;

namespace MusicJam.Domain.Entities;

public class PackRound : Entity
{
    public Guid PackId { get; set; }
    public Guid RoundId { get; set; }
    public int Order { get; set; }
    
    public RoundQuestion Round { get; set; } = null!;
    public Pack Pack { get; set; } = null!;
}