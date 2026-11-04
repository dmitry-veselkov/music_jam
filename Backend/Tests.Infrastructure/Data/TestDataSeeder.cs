using MusicJam.Studio.Infrustructure.Data;

namespace Tests.Infrastructure.Data;

public sealed class TestDataSeeder(AppDbContext dbContext)
{
    public async Task SeedAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await dbContext.Set<TEntity>().AddAsync(
            entity,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SeedRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await dbContext.Set<TEntity>().AddRangeAsync(
            entities,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SeedAsync(
        Action<AppDbContext> configure,
        CancellationToken cancellationToken = default)
    {
        configure(dbContext);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
