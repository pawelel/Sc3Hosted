using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3Hosted.Server.Entities;
namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateConfig : IEntityTypeConfiguration<Communicate>
{
    public void Configure(EntityTypeBuilder<Communicate> builder)
    {
        builder.HasKey(x => x.CommunicateId);
        builder.Property(x => x.CommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Scope).IsRequired();
    }
}
