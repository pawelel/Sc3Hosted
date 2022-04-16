using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class ModelParameterConfig : IEntityTypeConfiguration<ModelParameter>
{
    public void Configure(EntityTypeBuilder<ModelParameter> builder)
    {
        builder.ToTable("ModelParameters", x => x.IsTemporal());
        builder.HasKey(x => new { x.ModelId, x.ParameterId });
        builder.Property(x => x.ModelId).IsRequired();
        builder.Property(x => x.ParameterId).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(50);
        builder.HasOne(x => x.Model).WithMany(x => x.ModelParameters).HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Parameter).WithMany(x => x.ModelParameters).HasForeignKey(x => x.ParameterId).OnDelete(DeleteBehavior.Restrict);
    }
}
