using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class AreaConfig : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas", x => x.IsTemporal());
        builder.HasKey(x => x.AreaId);
        builder.Property(x => x.AreaId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();
        builder.HasMany(x => x.Spaces).WithOne(x => x.Area).HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.CommunicateAreas).WithOne(x => x.Area).HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Cascade);
    }
}
