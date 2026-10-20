using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Pack> Packs => Set<Pack>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PackRound> PackRounds => Set<PackRound>();
    public DbSet<QuestionUsages> QuestionUsages => Set<QuestionUsages>();
    public DbSet<RoundQuestion> RoundQuestions => Set<RoundQuestion>();
    
    public DbSet<Statistic> Statistics => Set<Statistic>();
    
    protected override void OnModelCreating(ModelBuilder b) =>
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}