namespace MusicJam.Runtime.Responses;

public class CreateGameResponse
{
    public Guid GameId { get; set; }
    public string InviteCode { get; set; }
    public string InviteLink { get; set; }
}