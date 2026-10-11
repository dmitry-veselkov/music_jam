using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations;

internal sealed class PackConfiguration : IEntityTypeConfiguration<Pack>
{
    public void Configure(EntityTypeBuilder<Pack> builder)
    {
        builder.ToTable("packs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId);
        builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
    }
}

internal sealed class PackRoundConfiguration : IEntityTypeConfiguration<PackRound>
{
    public void Configure(EntityTypeBuilder<PackRound> builder)
    {
        builder.ToTable("pack_round");
        builder.HasKey(x => new
        {
            x.PackId,
            x.RoundId
        });
        builder.HasOne(x => x.Round)
            .WithMany()
            .HasForeignKey(x => x.RoundId);
        builder.HasOne(x => x.Pack)
            .WithMany()
            .HasForeignKey(x => x.PackId);
    }
}