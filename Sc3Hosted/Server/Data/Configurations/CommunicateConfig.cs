using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Server.Data.Configurations;
public class CommunicateConfig : IEntityTypeConfiguration<Communicate>
{
    public void Configure(EntityTypeBuilder<Communicate> builder)
    {
        builder.ToTable("Communicates", x => x.IsTemporal());
        builder.HasKey(x => x.CommunicateId);
        builder.Property(x => x.CommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Scope).IsRequired();
        builder.HasMany(x => x.AssetCommunicates).WithOne(x => x.Communicate).HasForeignKey(x => x.CommunicateId);
        builder.HasMany(x=>x.AreaCommunicates).WithOne(x=>x.Communicate).HasForeignKey(x=>x.CommunicateId);
        builder.HasMany(x=>x.DeviceCommunicates).WithOne(x=>x.Communicate).HasForeignKey(x=>x.CommunicateId);
        builder.HasMany(x=>x.CoordinateCommunicates).WithOne(x=>x.Communicate).HasForeignKey(x=>x.CommunicateId);
builder.HasMany(x=>x.ModelCommunicates).WithOne(x=>x.Communicate).HasForeignKey(x=>x.CommunicateId);
builder.HasMany(x=>x.SpaceCommunicates).WithOne(x=>x.Communicate).HasForeignKey(x=>x.CommunicateId);
    }
       
}
