using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class DeviceCommunicateConfig : IEntityTypeConfiguration<DeviceCommunicate>
{
    public void Configure(EntityTypeBuilder<DeviceCommunicate> builder)
    {
        builder.ToTable("DeviceCommunicates", x => x.IsTemporal());
    }
}
