using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations;

internal sealed class StatisticsConfiguration : IEntityTypeConfiguration<Statistic>
{
    public void Configure(EntityTypeBuilder<Statistic> builder)
    {
        builder.ToTable("statistics");
        builder.HasKey(x => new
        {
            x.Id,
            x.UserId
        });
        builder.Property(x => x.UserId);
        builder.Property(x => x.LossesCount);
        builder.Property(x => x.WinsCount);
        builder.Property(x => x.TotalCount );

        builder.HasOne(x => x.User)
            .WithOne();
    }
}
