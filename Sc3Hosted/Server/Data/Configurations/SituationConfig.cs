using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class SituationConfig : IEntityTypeConfiguration<Situation>
{
    public void Configure(EntityTypeBuilder<Situation> builder)
    {
        builder.HasKey(x => x.SituationId);
        builder.Property(x => x.SituationId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
    }
}
