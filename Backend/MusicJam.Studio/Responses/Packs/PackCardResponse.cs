namespace MusicJam.Studio.Responses.Packs;

public class PackCardResponse
{
    public Guid PackId { get; set; }

    public string Name { get; set; }

    public string CreatedAtUtc { get; set; }

    public string UpdatedAtUtc { get; set; }
}
