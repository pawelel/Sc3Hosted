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
        builder.HasOne(x => x.Device).WithMany(x => x.CommunicateDevices).HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Communicate).WithMany(x => x.CommunicateDevices).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
    }
}
