namespace MusicJam.Studio.Models.Packs;

public class Pack
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Round> Rounds { get; set; }
}
