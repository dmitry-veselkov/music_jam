using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Studio.Infrastructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations;


internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Order).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
    }
}

