using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrastructure.Entities;

namespace MusicJam.Studio.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Pack> Packs => Set<Pack>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PackRound> PackRounds => Set<PackRound>();
    public DbSet<QuestionUsages> QuestionUsages => Set<QuestionUsages>();
    public DbSet<RoundQuestion> RoundQuestions => Set<RoundQuestion>();
    public DbSet<HistoryPack> HistoryPacks => Set<HistoryPack>();
    
    protected override void OnModelCreating(ModelBuilder b) =>
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}