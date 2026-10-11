using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations;

internal sealed class RoundConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.ToTable("rounds");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Questions).IsRequired();
    }
}