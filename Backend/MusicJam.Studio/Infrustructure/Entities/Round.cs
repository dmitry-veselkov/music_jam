namespace MusicJam.Studio.Infrastructure.Entities;


public class Round : Entity
{
    public string Title { get; set; } = string.Empty;
    
    public List<RoundQuestion> Questions { get; set; } = [];
}