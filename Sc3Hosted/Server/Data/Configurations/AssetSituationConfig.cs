using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class AssetSituationConfig : IEntityTypeConfiguration<AssetSituation>
{
    public void Configure(EntityTypeBuilder<AssetSituation> builder)
    {
        builder.ToTable("AssetSituations");

        builder.HasKey(x => new { x.AssetId, x.SituationId });
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.SituationId).IsRequired();
    }
}
