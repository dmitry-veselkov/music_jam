
using MusicJam.Domain.Common;

namespace MusicJam.Domain.Entities;

public class Pack : Entity
{
    
    public Guid UserId { get; set; }
    public String Title { get; set; } = string.Empty;
    public List<RoundQuestion> Rounds { get; set; } = [];
}