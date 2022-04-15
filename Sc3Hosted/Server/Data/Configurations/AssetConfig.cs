using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(x => x.AssetId);
        builder.Property(x => x.AssetId).ValueGeneratedOnAdd();
        builder.HasAlternateKey(x => x.Name);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.CoordinateId).IsRequired();
        builder.Property(x => x.ModelId).IsRequired();
        builder.HasOne(x => x.Model).WithMany(x => x.Assets).HasForeignKey(x => x.ModelId);
        builder.HasOne(x => x.Coordinate).WithMany(x => x.Assets).HasForeignKey(x => x.CoordinateId);
    }
}
