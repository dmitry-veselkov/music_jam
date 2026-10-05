namespace MusicJam.Domain.Models;

public class Pack
{
    public Guid Id { get; set; }
    public String Title;
    public List<Round> Rounds;
}