using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class SituationConfig : IEntityTypeConfiguration<Situation>
{
    public void Configure(EntityTypeBuilder<Situation> builder)
    {
        builder.ToTable("Situations", x => x.IsTemporal());
        builder.HasKey(x => x.SituationId);
        builder.Property(x => x.SituationId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.HasMany(x => x.SituationQuestions).WithOne(x => x.Situation).HasForeignKey(x => x.SituationId);
        builder.HasMany(x => x.SituationDetails).WithOne(x => x.Situation).HasForeignKey(x => x.SituationId);
        builder.HasMany(x => x.SituationParameters).WithOne(x => x.Situation).HasForeignKey(x => x.SituationId);
        builder.HasMany(x => x.CategorySituations).WithOne(x => x.Situation).HasForeignKey(x => x.SituationId);
        builder.HasMany(x => x.DeviceSituations).WithOne(x => x.Situation).HasForeignKey(x => x.SituationId);
    }
}
