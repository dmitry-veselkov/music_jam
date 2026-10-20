namespace MusicJam.Studio.Infrustructure.Entities;

public class QuestionUsages : Entity
{
    public Guid QuestionId { get; set; }
    public Guid PackId { get; set; }
    
    public Pack Pack { get; set; }
    public Question Question { get; set; }
}