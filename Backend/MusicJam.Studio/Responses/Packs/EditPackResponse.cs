namespace MusicJam.Studio.Responses.Packs;

public class EditPackResponse
{
    public Guid PackId { get; set; }

    public string Name { get; set; }

    public List<RoundResponse> Rounds { get; set; }
}
