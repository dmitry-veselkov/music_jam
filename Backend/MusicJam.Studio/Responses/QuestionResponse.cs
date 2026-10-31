namespace MusicJam.Studio.Responses;

public class QuestionResponse
{
    public Guid? QuestionId { get; set; }

    public string Category { get; set; }

    public int Cost { get; set; }

    public string? CorrectAnswer { get; set; }
}
