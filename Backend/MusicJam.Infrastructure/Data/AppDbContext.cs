using MusicJam.Domain.Common;
using Microsoft.EntityFrameworkCore;
using MusicJam.Domain.Entities;


namespace MusicJam.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Pack> Packs => Set<Pack>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PackRound> PackRounds => Set<PackRound>();
    public DbSet<QuestionUsage> QuestionUsages => Set<QuestionUsage>();
    public DbSet<RoundQuestion> RoundQuestions => Set<RoundQuestion>();
    
    protected override void OnModelCreating(ModelBuilder b) =>
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = now;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}