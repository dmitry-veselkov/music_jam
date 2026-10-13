using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations;

internal sealed class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("tracks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Link).IsRequired();
    }
}