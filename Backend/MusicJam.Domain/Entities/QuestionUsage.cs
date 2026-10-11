namespace MusicJam.Domain.Entities;

public class QuestionUsage
{
    public Guid QuestionId { get; set; }
    public Guid PackId { get; set; }
    
    public Pack Pack { get; set; }
    public Question Question { get; set; }
}