
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;

public class SituationQuestionConfig : IEntityTypeConfiguration<SituationQuestion>
{
    public void Configure(EntityTypeBuilder<SituationQuestion> builder)
    {
        builder.ToTable("SituationQuestions", x => x.IsTemporal());

    }
}