namespace MusicJam.Studio.Models.Responses;

public class RoundForTable
{
    public string Name { get; set; }
    public List<string> Categories { get; set; }
    public List<int> Costs { get; set; }
    public List<List<QuestionTableCell>> Questions { get; set; }
}
