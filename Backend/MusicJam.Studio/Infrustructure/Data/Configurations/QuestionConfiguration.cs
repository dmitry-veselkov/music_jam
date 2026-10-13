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
        builder.Property(x => x.QuestionTrackId).IsRequired();
        builder.Property(x => x.AnswerTrackId).IsRequired();
        builder.Property(x => x.Requested).IsRequired();
        builder.Property(x => x.CorrectAnswer).IsRequired();
        builder.Property(x => x.UserId).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.QuestionTrack)
            .WithMany()
            .HasForeignKey(x => x.QuestionTrackId);
        builder.HasOne(x => x.AnswerTrack)
            .WithMany()
            .HasForeignKey(x => x.AnswerTrackId);
    }
}

internal sealed class QuestionUsageConfiguration : IEntityTypeConfiguration<QuestionUsages>
{
    public void Configure(EntityTypeBuilder<QuestionUsages> builder)
    {
        builder.ToTable("question_usage");
        builder.HasKey(x => new
        {
            x.PackId,
            x.QuestionId
        });

        builder.HasOne(x => x.Pack)
            .WithMany()
            .HasForeignKey(x => x.PackId);
        
        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId);
    }
}