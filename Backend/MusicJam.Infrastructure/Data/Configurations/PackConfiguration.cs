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
        builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
    }
}