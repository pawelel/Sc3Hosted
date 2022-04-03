using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class ModelCommunicateConfig : IEntityTypeConfiguration<ModelCommunicate>
{
    public void Configure(EntityTypeBuilder<ModelCommunicate> builder)
    {
        builder.ToTable("ModelCommunicates", x => x.IsTemporal());
        builder.HasKey(x => x.ModelCommunicateId);
        builder.Property(x => x.ModelCommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.ModelId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
