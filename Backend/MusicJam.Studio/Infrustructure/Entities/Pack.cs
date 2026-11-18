namespace MusicJam.Studio.Infrastructure.Entities;

public class Pack : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public List<PackRound> Rounds { get; set; } = [];
}

public sealed record PackShortView(Guid Id, string Name);

public sealed record PackCardView(Guid Id, string Name, DateTime CreatedAt, DateTime UpdatedAt);