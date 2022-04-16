using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateCoordinateConfig : IEntityTypeConfiguration<CommunicateCoordinate>
{
    public void Configure(EntityTypeBuilder<CommunicateCoordinate> builder)
    {
        builder.ToTable("CommunicateCoordinates", x => x.IsTemporal());
        builder.HasKey(x => new { x.CommunicateId, x.CoordinateId });
        builder.Property(x => x.CoordinateId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
        builder.HasOne(x => x.Coordinate).WithMany(x => x.CommunicateCoordinates).HasForeignKey(x => x.CoordinateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Communicate).WithMany(x => x.CommunicateCoordinates).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
    }
}
