using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class ParameterConfig : IEntityTypeConfiguration<Parameter>
{
    public void Configure(EntityTypeBuilder<Parameter> builder)
    {
        builder.ToTable("Parameters", x => x.IsTemporal());
        builder.HasKey(x => x.ParameterId);
        builder.Property(x => x.ParameterId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.HasMany(x => x.ModelParameters).WithOne(x => x.Parameter).HasForeignKey(x => x.ParameterId);
        builder.HasMany(x => x.SituationParameters).WithOne(x => x.Parameter).HasForeignKey(x => x.ParameterId);
    }
}
