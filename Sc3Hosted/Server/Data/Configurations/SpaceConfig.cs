using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Server.Data.Configurations;

public class SpaceConfig : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder.ToTable("Spaces", x => x.IsTemporal());

    }
}
