using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateSpaceConfig : IEntityTypeConfiguration<CommunicateSpace>
{
    public void Configure(EntityTypeBuilder<CommunicateSpace> builder)
    {
        builder.ToTable("CommunicateSpaces", x => x.IsTemporal());
        builder.HasKey(x => new { x.CommunicateId, x.SpaceId });
        builder.Property(x => x.SpaceId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
        builder.HasOne(x => x.Communicate).WithMany(x => x.CommunicateSpaces).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Space).WithMany(x => x.CommunicateSpaces).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.Restrict);
    }
}
