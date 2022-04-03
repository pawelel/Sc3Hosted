using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class DetailConfig : IEntityTypeConfiguration<Detail>
{
    public void Configure(EntityTypeBuilder<Detail> builder)
    {
        builder.ToTable("Details", x => x.IsTemporal());
        builder.HasKey(x => x.DetailId);
        builder.Property(x => x.DetailId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasMany(x => x.SituationDetails).WithOne(x => x.Detail).HasForeignKey(x => x.DetailId);
        builder.HasMany(x => x.AssetDetails).WithOne(x => x.Detail).HasForeignKey(x => x.DetailId);
    }
}
