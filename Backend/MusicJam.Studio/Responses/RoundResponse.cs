namespace MusicJam.Studio.Responses;

public class RoundResponse
{
    public string Name { get; set; }

    public List<string> Categories { get; set; } = [];

    public List<int> Costs { get; set; } = [];

    public List<List<QuestionResponse>> Questions { get; set; } = [];
}
