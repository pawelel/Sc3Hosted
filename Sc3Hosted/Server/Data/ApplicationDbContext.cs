

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data.Configurations;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>()
               .HasData(new IdentityRole { Name = "User", NormalizedName = "USER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
        builder.Entity<IdentityRole>()
               .HasData(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
        builder.Entity<IdentityRole>()
               .HasData(new IdentityRole { Name = "Manager", NormalizedName = "MANAGER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
        
        builder.Entity<Plant>().ToTable("Plants", x => {
            x.IsTemporal();
            builder.Entity<Area>().ToTable("Areas", x =>
            {
                x.IsTemporal();
            });
            
                builder.Entity<Area>().HasOne(a => a.Plant).WithMany(p => p.Areas).HasForeignKey(p => p.AreaId);
                builder.Entity<AreaCommunicate>().HasOne(a => a.Area).WithMany(p => p.AreaCommunicates).HasForeignKey(p => p.AreaCommunicateId);
            builder.Entity<AreaCommunicate>().HasOne(a => a.Communicate).WithMany(p => p.AreaCommunicates).HasForeignKey(p => p.CommunicateId);
            builder.Entity<AssetCommunicate>().ToTable("AssetCommunicates", x =>
            {
                x.IsTemporal();
            })
            .HasOne(a => a.Asset).WithMany(p => p.AssetCommunicates).HasForeignKey(p => p.AssetId);

        });
    }

    // information
    public DbSet<AreaCommunicate> AreaCommunicates =>Set<AreaCommunicate>();
    public DbSet<AssetCommunicate> AssetCommunicates =>Set<AssetCommunicate>();
    public DbSet<Communicate> Communicates =>Set<Communicate>();
    public DbSet<CoordinateCommunicate> CoordinateCommunicates =>Set<CoordinateCommunicate>();
    public DbSet<DeviceCommunicate> DeviceCommunicates =>Set<DeviceCommunicate>();
    public DbSet<ModelCommunicate> ModelCommunicates =>Set<ModelCommunicate>();
    public DbSet<SpaceCommunicate> SpaceCommunicates =>Set<SpaceCommunicate>();

    // location
    public DbSet<Area> Areas =>Set<Area>();
    public DbSet<Coordinate> Coordinates =>Set<Coordinate>();
    public DbSet<Space> Spaces =>Set<Space>();

    // occurence
    public DbSet<SituationAsset> SituationAssets =>Set<SituationAsset>();
    public DbSet<SituationCategory> SituationCategories =>Set<SituationCategory>();
    public DbSet<Question> Questions =>Set<Question>();
    public DbSet<Situation> Situations =>Set<Situation>();
    public DbSet<SituationQuestion> SituationQuestions =>Set<SituationQuestion>();
    public DbSet<SituationDetail> SituationDetails =>Set<SituationDetail>();
    public DbSet<SituationParameter> SituationParameters =>Set<SituationParameter>();

    //stuff
    public DbSet<Asset> Assets =>Set<Asset>();
    public DbSet<AssetCategoryConfig> AssetCategories =>Set<AssetCategoryConfig>();
    public DbSet<AssetDetail> AssetDetails =>Set<AssetDetail>();
    public DbSet<Category> Categories =>Set<Category>();
    public DbSet<Detail> Details =>Set<Detail>();
    public DbSet<Device> Devices =>Set<Device>();
    public DbSet<Model> Models =>Set<Model>();
    public DbSet<ModelParameter> ModelParameters =>Set<ModelParameter>();
    public DbSet<Parameter> Parameters =>Set<Parameter>();
    public DbSet<Plant> Plants =>Set<Plant>();
}
