namespace MusicJam.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public Guid QuestionTrackId { get; set; }
    public Guid AnswerTrackId { get; set; }
    public string Description { get; set; } = String.Empty;
    public string AnswerStr { get; set; } = String.Empty;
    
    public User User { get; set; } = null!;
    public Track QuestionTrack { get; set; } = null!;
    public Track AnswerTrack { get; set; } = null!;


}