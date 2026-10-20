
namespace MusicJam.Studio.Infrustructure.Entities;

public class Question : Entity
{
    
    public Guid UserId { get; set; }
    
    public Guid Id { get; set; }

    public Guid QuestionTrackId { get; set; }
    public Guid AnswerTrackId { get; set; }
    public string Requested { get; set; } = String.Empty;
    public string CorrectAnswer { get; set; } = String.Empty;
    
    public User User { get; set; } = null!;
    public Track QuestionTrack { get; set; } = null!;
    public Track AnswerTrack { get; set; } = null!;


}