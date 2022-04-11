using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Enumerations;

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
        builder.Property(x=>x.CoordinateId).IsRequired();
        builder.Property(x=>x.ModelId).IsRequired();
    }
}