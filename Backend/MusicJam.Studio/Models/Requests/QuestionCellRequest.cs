namespace MusicJam.Studio.Models.Requests;

public class QuestionCellRequest
{
    public string Category { get; set; }
    public int Cost { get; set; }
    public Guid QuestionId { get; set; }
}
