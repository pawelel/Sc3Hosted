using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data.Configurations;

public class ModelParameterConfig : IEntityTypeConfiguration<ModelParameter>
{
    public void Configure(EntityTypeBuilder<ModelParameter> builder)
    {
        builder.ToTable("ModelParameters", x => x.IsTemporal());

    }
}