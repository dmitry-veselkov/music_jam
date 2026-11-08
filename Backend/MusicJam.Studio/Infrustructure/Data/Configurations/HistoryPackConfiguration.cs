using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Studio.Infrastructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations;

internal sealed class HistoryPackConfiguration : IEntityTypeConfiguration<HistoryPack>
{
    public void Configure(EntityTypeBuilder<HistoryPack> builder)
    {
        builder.ToTable("history_pack");
        builder.HasKey(x => x.PackId);
        builder.Property(x => x.Date);

        builder.HasOne<Pack>()
            .WithMany()
            .HasForeignKey(x => x.PackId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}