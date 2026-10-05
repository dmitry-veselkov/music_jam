namespace MusicJam.Domain.Models;

public class Question
{
    public Guid Id { get; set; }
    public Track question;
    public Track answer;
    public String description;
    public String answerStr;
    public List<String> usages;
}