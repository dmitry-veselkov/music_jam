
namespace MusicJam.Domain.Entities;

public class Round
{
    public Guid Id { get; set; }
    public int RoundOrder { get; set; }
    public List<RoundQuestion> Cells { get; set; } = [];
}