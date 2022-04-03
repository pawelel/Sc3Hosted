using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class CoordinateCommunicateConfig : IEntityTypeConfiguration<CoordinateCommunicate>
{
    public void Configure(EntityTypeBuilder<CoordinateCommunicate> builder)
    {
        builder.ToTable("CoordinateCommunicates", x => x.IsTemporal());
        builder.HasKey(x => x.CoordinateCommunicateId);
        builder.Property(x => x.CoordinateCommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.CoordinateId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
