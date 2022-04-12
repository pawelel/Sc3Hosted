using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets", x => x.IsTemporal());
        builder.HasKey(x => x.AssetId);
        builder.Property(x => x.AssetId).ValueGeneratedOnAdd();
        builder.HasAlternateKey(x => x.Name);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.CoordinateId).IsRequired();
        builder.Property(x => x.ModelId).IsRequired();
        builder.HasMany(x=>x.AssetCategories).WithOne(x=>x.Asset).HasForeignKey(x=>x.AssetId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x=>x.AssetDetails).WithOne(x=>x.Asset).HasForeignKey(x=>x.AssetId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x=>x.AssetSituations).WithOne(x=>x.Asset).HasForeignKey(x=>x.AssetId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x=>x.CommunicateAssets).WithOne(x=>x.Asset).HasForeignKey(x=>x.AssetId).OnDelete(DeleteBehavior.NoAction);
    }
}
