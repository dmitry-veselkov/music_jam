namespace MusicJam.Studio.Models.Responses;

public class QuestionTableCell
{
    public int Cost { get; set; }
    public string Category { get; set; }
    public Guid QuestionId { get; set; }
    public string CorrectAnswer { get; set; }
}
