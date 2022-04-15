using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class AssetDetailConfig : IEntityTypeConfiguration<AssetDetail>
{
    public void Configure(EntityTypeBuilder<AssetDetail> builder)
    {
        builder.HasKey(x => new { x.AssetId, x.DetailId });
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.DetailId).IsRequired();
        builder.HasOne(x => x.Asset).WithMany(x => x.AssetDetails).HasForeignKey(x => x.AssetId);
        builder.HasOne(x => x.Detail).WithMany(x => x.AssetDetails).HasForeignKey(x => x.DetailId);
    }
}
