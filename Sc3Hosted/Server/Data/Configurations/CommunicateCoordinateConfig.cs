using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateCoordinateConfig : IEntityTypeConfiguration<CommunicateCoordinate>
{
    public void Configure(EntityTypeBuilder<CommunicateCoordinate> builder)
    {
        builder.ToTable("CommunicateCoordinates", x => x.IsTemporal());
        builder.HasKey(x => new{x.CommunicateId, x.CoordinateId});
        builder.Property(x => x.CoordinateId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
