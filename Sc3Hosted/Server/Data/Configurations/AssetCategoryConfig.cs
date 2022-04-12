using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class AssetCategoryConfig : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("AssetCategories", x => x.IsTemporal());
        builder.HasKey(x => new { x.AssetId, x.CategoryId });
        builder.HasOne(x => x.Asset).WithMany(x => x.AssetCategories).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(x => x.Category).WithMany(x => x.AssetCategories).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.ClientCascade);
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();
    }
}
