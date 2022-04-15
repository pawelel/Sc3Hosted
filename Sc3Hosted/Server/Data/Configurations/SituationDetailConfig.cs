using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class SituationDetailConfig : IEntityTypeConfiguration<SituationDetail>
{
    public void Configure(EntityTypeBuilder<SituationDetail> builder)
    {
        builder.HasKey(x => new { x.SituationId, x.DetailId });
        builder.Property(x => x.SituationId).IsRequired();
        builder.Property(x => x.DetailId).IsRequired();
        builder.HasOne(x => x.Situation).WithMany(x => x.SituationDetails).HasForeignKey(x => x.SituationId);
        builder.HasOne(x => x.Detail).WithMany(x => x.SituationDetails).HasForeignKey(x => x.DetailId);
    }
}
