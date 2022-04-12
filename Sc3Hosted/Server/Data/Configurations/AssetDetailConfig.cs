using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class AssetDetailConfig : IEntityTypeConfiguration<AssetDetail>
{
    public void Configure(EntityTypeBuilder<AssetDetail> builder)
    {
        builder.ToTable("AssetDetails", x => x.IsTemporal());
        builder.HasKey(x => new { x.AssetId, x.DetailId });
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.DetailId).IsRequired();
    }
}
