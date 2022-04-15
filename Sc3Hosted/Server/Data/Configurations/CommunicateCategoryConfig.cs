using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateCategoryConfig : IEntityTypeConfiguration<CommunicateCategory>
{
    public void Configure(EntityTypeBuilder<CommunicateCategory> builder)
    {
        builder.HasKey(x => new { x.CommunicateId, x.CategoryId });
        builder.Property(x => x.CommunicateId).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();
        builder.HasOne(x => x.Communicate).WithMany(x => x.CommunicateCategories).HasForeignKey(x => x.CommunicateId);
        builder.HasOne(x => x.Category).WithMany(x => x.CommunicateCategories).HasForeignKey(x => x.CategoryId);
    }
}
