using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;

public class SituationCategoryConfig : IEntityTypeConfiguration<SituationCategory>
{
    public void Configure(EntityTypeBuilder<SituationCategory> builder)
    {
        builder.ToTable("SituationCategories", x => x.IsTemporal());
        builder.HasKey(x => x.SituationCategoryId);
        builder.Property(x => x.SituationCategoryId).ValueGeneratedOnAdd();
        builder.Property(x => x.SituationId).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();

    }
}