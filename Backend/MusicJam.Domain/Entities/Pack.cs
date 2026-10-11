
namespace MusicJam.Domain.Entities;

public class Pack
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public String Title { get; set; } = string.Empty;
    public List<Round> Rounds { get; set; } = [];
}