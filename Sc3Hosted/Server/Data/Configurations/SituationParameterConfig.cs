using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class SituationParameterConfig : IEntityTypeConfiguration<SituationParameter>
{
    public void Configure(EntityTypeBuilder<SituationParameter> builder)
    {
        builder.ToTable("SituationParameters", x => x.IsTemporal());
        builder.HasKey(x => new { x.SituationId, x.ParameterId });
        builder.Property(x => x.SituationId).IsRequired();
        builder.Property(x => x.ParameterId).IsRequired();
        builder.HasOne(x=>x.Parameter).WithMany(x=>x.SituationParameters).HasForeignKey(x=>x.ParameterId).OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(x=>x.Situation).WithMany(x=>x.SituationParameters).HasForeignKey(x=>x.SituationId).OnDelete(DeleteBehavior.ClientCascade);
    }
}
