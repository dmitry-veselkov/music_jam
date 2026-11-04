namespace MusicJam.Studio.Responses.Questions;

public class QuestionCardResponse
{
    public Guid QuestionId { get; set; }
    public string Requested { get; set; }
    public string CorrectAnswer { get; set; }
}
