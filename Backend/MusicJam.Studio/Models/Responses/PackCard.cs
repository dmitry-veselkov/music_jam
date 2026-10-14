namespace MusicJam.Studio.Models.Responses;

public class PackCard
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public List<GameShort> Usages { get; set; }
}
