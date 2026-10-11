namespace MusicJam.Domain.Entities;

public class RoundQuestion
{
    public Guid RoundId { get; set; }
    public Guid CategoryId { get; set; }
    public int Cost { get; set; }
    public Guid QuestionId { get; set; }
    
    public Round Round { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Question Question { get; set; } = null!;
}