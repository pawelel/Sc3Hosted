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
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();
    }
}
