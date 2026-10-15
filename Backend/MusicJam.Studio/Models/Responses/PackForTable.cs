namespace MusicJam.Studio.Models.Responses;

public class PackForTable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<RoundForTable> Rounds { get; set; }
}
