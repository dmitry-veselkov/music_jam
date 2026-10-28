using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicJam.Studio.Infrustructure.Entities;

internal sealed class RoundQuestionConfiguration : IEntityTypeConfiguration<RoundQuestion>
{
    public void Configure(EntityTypeBuilder<RoundQuestion> builder)
    {
        builder.ToTable("round_question");
        builder.HasKey(x => new {x.RoundId, x.CategoryId, x.Cost});

        builder.HasOne(x => x.Round)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.RoundId);

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired(false);
    }
}