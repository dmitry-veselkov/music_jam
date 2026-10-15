namespace MusicJam.Studio.Models.Responses;

public class RoundEdit
{
    public string Name { get; set; }
    public List<string> Categories { get; set; }
    public List<int> Costs { get; set; }
    public List<List<QuestionEditCell>> Questions { get; set; }
}
