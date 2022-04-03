using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;

public class ModelParameterConfig : IEntityTypeConfiguration<ModelParameter>
{
    public void Configure(EntityTypeBuilder<ModelParameter> builder)
    {
        builder.ToTable("ModelParameters", x => x.IsTemporal());
        builder.HasKey(x => x.ModelParameterId);
        builder.Property(x => x.ModelParameterId).ValueGeneratedOnAdd();
        builder.Property(x => x.ModelId).IsRequired();
        builder.Property(x => x.ParameterId).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(50);
    }
}