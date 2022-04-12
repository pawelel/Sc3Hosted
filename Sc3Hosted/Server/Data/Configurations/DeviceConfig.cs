using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class DeviceConfig : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices", x => x.IsTemporal());
        builder.HasKey(x => x.DeviceId);
        builder.Property(x => x.DeviceId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasMany(x => x.Models).WithOne(x => x.Device).HasForeignKey(x => x.DeviceId);
        builder.HasMany(x => x.CommunicateDevices).WithOne(x => x.Device).HasForeignKey(x => x.DeviceId);
    }
}
