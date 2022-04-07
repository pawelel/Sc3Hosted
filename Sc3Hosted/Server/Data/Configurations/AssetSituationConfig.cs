using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;


public class AssetSituationConfig : IEntityTypeConfiguration <AssetSituation>
{
    public void Configure(EntityTypeBuilder<AssetSituation> builder)
    {
        builder.ToTable("AssetSituations");

        builder.HasKey(x => x.AssetSituationId);

        builder.Property(x => x.AssetSituationId).ValueGeneratedOnAdd();

        builder.Property(x => x.AssetId).IsRequired();

        builder.Property(x => x.SituationId).IsRequired();

        builder.HasOne(x => x.Asset).WithMany(x => x.AssetSituations).HasForeignKey(x => x.AssetId);

        builder.HasOne(x => x.Situation).WithMany(x => x.AssetSituations).HasForeignKey(x => x.SituationId);
    }
}
