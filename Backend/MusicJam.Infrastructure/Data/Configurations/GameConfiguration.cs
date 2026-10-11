using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations;

internal sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Pack);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
        builder.Property(x => x.GameTime);
        builder.Property(x => x.Mode);
        builder.Property(x => x.State).IsRequired();
    }
}