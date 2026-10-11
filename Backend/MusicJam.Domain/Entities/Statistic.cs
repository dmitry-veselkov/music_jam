
namespace MusicJam.Domain.Entities;

public class Statistic
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int AmountWins { get; set; }
    public int AmountLosses { get; set; }
    public int HowManyGamesPlayed { get; set; }
    
    public User User { get; set; }
}