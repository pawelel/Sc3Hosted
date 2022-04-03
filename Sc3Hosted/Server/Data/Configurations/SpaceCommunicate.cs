using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class SpaceCommunicateConfig : IEntityTypeConfiguration<SpaceCommunicate>
{
    public void Configure(EntityTypeBuilder<SpaceCommunicate> builder)
    {
        builder.ToTable("SpaceCommunicates", x => x.IsTemporal());
        builder.HasKey(x => x.SpaceCommunicateId);
        builder.Property(x => x.SpaceCommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.SpaceId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
