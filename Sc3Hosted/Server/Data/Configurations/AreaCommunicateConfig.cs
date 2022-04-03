using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sc3Hosted.Server.Entities;
public class AreaCommunicateConfig : IEntityTypeConfiguration<AreaCommunicate>
{
    public void Configure(EntityTypeBuilder<AreaCommunicate> builder)
    {
        builder.ToTable("AreaCommunicates", x=>x.IsTemporal());
        builder.HasKey(x => x.AreaCommunicateId);
        builder.Property(x => x.AreaCommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.AreaId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
