using MusicJam.Music;

namespace MusicJam.Studio.Models.Packs;

public class Question
{
    public Guid Id { get; set; }
    public string Requested { get; set; }
    public string CorrectAnswer { get; set; }
    public Track QuestionTrack { get; set; }
    public Track AnswerTrack { get; set; }
    
    public List<Pack> Usages { get; set; }
}
