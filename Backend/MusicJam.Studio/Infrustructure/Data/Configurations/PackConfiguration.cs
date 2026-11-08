using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Studio.Infrastructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations;

internal sealed class PackConfiguration : IEntityTypeConfiguration<Pack>
{
    public void Configure(EntityTypeBuilder<Pack> builder)
    {
        builder.ToTable("packs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(255).IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("now()");
        
        builder.HasOne<User>()
            .WithMany(x => x.Packs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    internal sealed class PackRoundConfiguration : IEntityTypeConfiguration<PackRound>
    {
        public void Configure(EntityTypeBuilder<PackRound> builder)
        {
            builder.ToTable("pack_round");
            builder.HasKey(x => new { x.PackId, x.RoundId });
            builder.Property(x => x.Order).IsRequired();
            
            builder.HasOne(x => x.Round)
                .WithMany()
                .HasForeignKey(x => x.RoundId);
            
            builder.HasOne(x => x.Pack)
                .WithMany(x => x.Rounds)
                .HasForeignKey(x => x.PackId);
        }
    }
}