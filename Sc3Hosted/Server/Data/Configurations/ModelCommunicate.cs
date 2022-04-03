using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class ModelCommunicateConfig : IEntityTypeConfiguration<ModelCommunicate>
{
    public void Configure(EntityTypeBuilder<ModelCommunicate> builder)
    {
        builder.ToTable("ModelCommunicates", x => x.IsTemporal());
    }
}
