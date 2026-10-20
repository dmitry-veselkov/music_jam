namespace MusicJam.Studio.Infrustructure.Entities;
public class PackRound : Entity
{
    public Guid PackId { get; set; }
    public Guid RoundId { get; set; }
    public int Order { get; set; }
    
    public Pack Pack { get; set; } = null!;
    public Round Round { get; set; } = null!;
}