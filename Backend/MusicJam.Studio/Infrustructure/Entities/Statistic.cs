
using MusicJam.Domain.Common;

namespace MusicJam.Domain.Entities;

public class Statistic : Entity
{
    public Guid UserId { get; set; }
    public int WinsCount { get; set; }
    public int LossesCount { get; set; }
    public int TotalCount  { get; set; }
    
    public User User { get; set; }
}