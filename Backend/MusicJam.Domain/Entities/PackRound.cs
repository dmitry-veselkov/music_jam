namespace MusicJam.Domain.Entities;

public class PackRound
{
    public Guid PackId { get; set; }
    public Guid RoundId { get; set; }
    public int Order { get; set; }
    
    public RoundQuestion Round { get; set; } = null!;
    public Pack Pack { get; set; } = null!;
}