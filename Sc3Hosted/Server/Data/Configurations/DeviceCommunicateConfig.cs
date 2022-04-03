using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class DeviceCommunicateConfig : IEntityTypeConfiguration<DeviceCommunicate>
{
    public void Configure(EntityTypeBuilder<DeviceCommunicate> builder)
    {
        builder.ToTable("DeviceCommunicates", x => x.IsTemporal());
        builder.HasKey(x => x.DeviceCommunicateId);
        builder.Property(x => x.DeviceCommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.DeviceId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
