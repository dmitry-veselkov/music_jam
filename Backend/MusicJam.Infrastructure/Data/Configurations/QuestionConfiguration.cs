using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("questions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.QuestionTrack).IsRequired();
        builder.Property(x => x.AnswerTrack).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.AnswerStr).IsRequired();
        builder.Property(x => x.AnswerStr).IsRequired();
    }
}