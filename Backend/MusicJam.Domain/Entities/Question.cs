namespace MusicJam.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Track QuestionTrack { get; set; }
    public Track AnswerTrack { get; set; }
    public string Description { get; set; } = String.Empty;
    public string AnswerStr { get; set; } = String.Empty;
    public List<string> Usages { get; set; } = [];
}