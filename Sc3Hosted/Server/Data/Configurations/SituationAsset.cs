using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;

public class SituationAssetConfig : IEntityTypeConfiguration<SituationAsset>
{
    public void Configure(EntityTypeBuilder<SituationAsset> builder)
    {
        builder.ToTable("SituationAssets", x => x.IsTemporal());
        builder.HasKey(x => x.SituationAssetId);
        builder.Property(x => x.SituationAssetId).ValueGeneratedOnAdd();
        builder.Property(x => x.SituationId).IsRequired();
        builder.Property(x => x.AssetId).IsRequired();
    }
}