using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class PlantConfig : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.ToTable("Plants", x => x.IsTemporal());
        builder.HasKey(x => x.PlantId);
        builder.Property(x => x.PlantId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);

        builder.HasMany(x => x.Areas).WithOne(x => x.Plant).HasForeignKey(x => x.PlantId);

    }
}
