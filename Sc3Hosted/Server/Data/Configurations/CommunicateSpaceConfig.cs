using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateSpaceConfig : IEntityTypeConfiguration<CommunicateSpace>
{
    public void Configure(EntityTypeBuilder<CommunicateSpace> builder)
    {
        builder.ToTable("CommunicateSpaces", x => x.IsTemporal());
        builder.HasKey(x => new{x.CommunicateId, x.SpaceId});
        builder.Property(x => x.SpaceId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
