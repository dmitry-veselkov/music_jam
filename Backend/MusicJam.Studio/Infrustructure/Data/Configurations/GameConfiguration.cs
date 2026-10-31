using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicJam.Studio.Infrustructure.Entities;

internal sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId);
        builder.Property(x => x.PackId);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
        builder.Property(x => x.GameTime);
        builder.Property(x => x.Mode);
        builder.Property(x => x.State).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Pack)
            .WithMany()
            .HasForeignKey(x => x.PackId);
    }
}