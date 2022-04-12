using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class SituationQuestionConfig : IEntityTypeConfiguration<SituationQuestion>
{
    public void Configure(EntityTypeBuilder<SituationQuestion> builder)
    {
        builder.ToTable("SituationQuestions", x => x.IsTemporal());
        builder.HasKey(x => new { x.SituationId, x.QuestionId });
        builder.Property(x => x.SituationId).IsRequired();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.HasOne(x => x.Question).WithMany(x => x.SituationQuestions).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(x => x.Situation).WithMany(x => x.SituationQuestions).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.ClientCascade);
    }
}
