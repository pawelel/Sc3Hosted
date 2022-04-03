
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class AssetCategoryConfig : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("AssetCategories", x=>x.IsTemporal());
        builder.HasKey(x => x.AssetCategoryId);
        builder.Property(x => x.AssetCategoryId).ValueGeneratedOnAdd();
    }
}
