namespace MusicJam.Studio.Models.Responses;

public class PackEdit
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<RoundEdit> Rounds { get; set; }
}
