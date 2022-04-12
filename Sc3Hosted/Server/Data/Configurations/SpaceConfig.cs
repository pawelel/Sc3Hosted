using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class SpaceConfig : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder.ToTable("Spaces", x => x.IsTemporal());
        builder.HasKey(x => x.SpaceId);
        builder.Property(x => x.SpaceId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.HasOne(x=>x.Area).WithMany(x=>x.Spaces).HasForeignKey(x=>x.AreaId).OnDelete(DeleteBehavior.ClientCascade);
        builder.HasMany(x => x.Coordinates).WithOne(x => x.Space).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.CommunicateSpaces).WithOne(x => x.Space).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.NoAction);
    }
}
