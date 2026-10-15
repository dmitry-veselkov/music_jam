namespace MusicJam.Studio.Models.Requests;

public class LoadPackRequest
{
    public string Name { get; set; }
    public List<RoundRequest> Rounds { get; set; }
}
