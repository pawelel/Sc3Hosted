using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateDeviceConfig : IEntityTypeConfiguration<CommunicateDevice>
{
    public void Configure(EntityTypeBuilder<CommunicateDevice> builder)
    {
        builder.ToTable("CommunicateDevices", x => x.IsTemporal());
        builder.HasKey(x => new { x.CommunicateId, x.DeviceId });
        builder.Property(x => x.DeviceId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
