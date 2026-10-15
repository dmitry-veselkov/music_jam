namespace MusicJam.Studio.Models.Packs;

public class Round
{
    public string Name { get; set; }
    public Dictionary<string, Dictionary<int, Guid>> QuestionIds { get; set; }
}
