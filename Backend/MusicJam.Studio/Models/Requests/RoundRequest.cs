namespace MusicJam.Studio.Models.Requests;

public class RoundRequest
{
    public string Name { get; set; }
    public List<List<QuestionCellRequest>> Questions { get; set; }
}
