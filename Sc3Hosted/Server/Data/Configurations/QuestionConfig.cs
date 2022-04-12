using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class QuestionConfig : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions", x => x.IsTemporal());
        builder.HasKey(x => x.QuestionId);
        builder.Property(x => x.QuestionId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
    }
}
