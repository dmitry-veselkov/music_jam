using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrastructure.Data;

namespace Tests.Infrastructure.Database;

public sealed class DatabaseInitializer(AppDbContext dbContext)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
