
namespace MusicJam.Domain.Entities;

public class Round
{
    public Guid Id { get; set; }
    public int RoundOrder { get; set; }
    public Dictionary<(Category, int), Question>? Questions { get; set; } = [];
}