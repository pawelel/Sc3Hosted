using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Sc3Hosted.Server.Entities;
public class CommunicateAreaConfig : IEntityTypeConfiguration<CommunicateArea>
{
    public void Configure(EntityTypeBuilder<CommunicateArea> builder)
    {
        builder.HasKey(x => new { x.CommunicateId, x.AreaId });
        builder.Property(x => x.AreaId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
        builder.HasOne(x => x.Communicate).WithMany(x => x.CommunicateAreas).HasForeignKey(x => x.CommunicateId);
        builder.HasOne(x => x.Area).WithMany(x => x.CommunicateAreas).HasForeignKey(x => x.AreaId);
    }
}
