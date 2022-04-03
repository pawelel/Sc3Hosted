using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;

public class SituationCategoryConfig : IEntityTypeConfiguration<SituationCategory>
{
    public void Configure(EntityTypeBuilder<SituationCategory> builder)
    {
        builder.ToTable("SituationCategories", x => x.IsTemporal());

    }
}