using MusicJam.Studio.Models.Games;

namespace MusicJam.Studio.Models.Packs;

public class Pack
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Round> Rounds { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public List<Game> Usages { get; set; }
}
