using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicJam.Studio.Infrustructure.Entities;

internal sealed class StatisticsConfiguration : IEntityTypeConfiguration<Statistic>
{
    public void Configure(EntityTypeBuilder<Statistic> builder)
    {
        builder.ToTable("statistics");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId);
        builder.Property(x => x.LossesCount);
        builder.Property(x => x.WinsCount);
        builder.Property(x => x.TotalCount );

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Statistic>(x => x.UserId);
    }
}
