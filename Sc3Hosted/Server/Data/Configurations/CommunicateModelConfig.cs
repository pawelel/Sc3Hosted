using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateModelConfig : IEntityTypeConfiguration<CommunicateModel>
{
    public void Configure(EntityTypeBuilder<CommunicateModel> builder)
    {
        builder.ToTable("CommunicateModels", x => x.IsTemporal());
        builder.HasKey(x => x.CommunicateModelId);
        builder.Property(x => x.CommunicateModelId).ValueGeneratedOnAdd();
        builder.Property(x => x.ModelId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
    }
}
