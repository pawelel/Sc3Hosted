using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class AssetCommunicateConfig : IEntityTypeConfiguration<AssetCommunicate>
{
    public void Configure(EntityTypeBuilder<AssetCommunicate> builder)
    {
        builder.ToTable("AssetCommunicates", x => x.IsTemporal());
        builder.HasKey(x => x.AssetCommunicateId);
        builder.Property(x => x.AssetCommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
