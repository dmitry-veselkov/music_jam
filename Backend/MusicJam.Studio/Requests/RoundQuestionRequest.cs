namespace MusicJam.Studio.Requests;

public class RoundQuestionRequest
{
    public int Cost { get; set; }

    public string Category { get; set; } = string.Empty;

    public Guid? QuestionId { get; set; }
}
