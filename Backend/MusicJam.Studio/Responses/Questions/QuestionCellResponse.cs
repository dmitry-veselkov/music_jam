namespace MusicJam.Studio.Responses.Questions;

public class QuestionCellResponse
{
    public Guid? QuestionId { get; set; }

    public string Category { get; set; }

    public int Cost { get; set; }

    public string? CorrectAnswer { get; set; }
}
