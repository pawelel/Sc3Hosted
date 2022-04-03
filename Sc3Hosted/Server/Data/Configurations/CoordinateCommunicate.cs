using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class CoordinateCommunicateConfig : IEntityTypeConfiguration<CoordinateCommunicate>
{
    public void Configure(EntityTypeBuilder<CoordinateCommunicate> builder)
    {
        builder.ToTable("CoordinateCommunicates", x => x.IsTemporal());
    }
}
