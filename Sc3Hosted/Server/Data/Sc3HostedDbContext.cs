

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data.Configurations;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Data;

public class Sc3HostedDbContext : IdentityDbContext<ApplicationUser>
{
    public Sc3HostedDbContext(DbContextOptions options) : base(options)
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

        new AreaConfig().Configure(builder.Entity<Area>());
        new CommunicateAreaConfig().Configure(builder.Entity<CommunicateArea>());
        new CommunicateAssetConfig().Configure(builder.Entity<CommunicateAsset>());
        new CommunicateConfig().Configure(builder.Entity<Communicate>());
        new CommunicateCoordinateConfig().Configure(builder.Entity<CommunicateCoordinate>());
        new CommunicateDeviceConfig().Configure(builder.Entity<CommunicateDevice>());
        new CommunicateModelConfig().Configure(builder.Entity<CommunicateModel>());
        new CommunicateSpaceConfig().Configure(builder.Entity<CommunicateSpace>());
        new AreaConfig().Configure(builder.Entity<Area>());
        new CoordinateConfig().Configure(builder.Entity<Coordinate>());
        new SpaceConfig().Configure(builder.Entity<Space>());
        new SituationAssetConfig().Configure(builder.Entity<SituationAsset>());
        new SituationCategoryConfig().Configure(builder.Entity<SituationCategory>());
        new SituationConfig().Configure(builder.Entity<Situation>());
        new QuestionConfig().Configure(builder.Entity<Question>());
        new SituationQuestionConfig().Configure(builder.Entity<SituationQuestion>());
        new SituationDetailConfig().Configure(builder.Entity<SituationDetail>());
        new SituationParameterConfig().Configure(builder.Entity<SituationParameter>());
        new AssetConfig().Configure(builder.Entity<Asset>());
        new AssetCategoryConfig().Configure(builder.Entity<AssetCategory>());
        new AssetDetailConfig().Configure(builder.Entity<AssetDetail>());
        new CategoryConfig().Configure(builder.Entity<Category>());
        new DeviceConfig().Configure(builder.Entity<Device>());
        new DetailConfig().Configure(builder.Entity<Detail>());
        new ModelConfig().Configure(builder.Entity<Model>());
        new ModelParameterConfig().Configure(builder.Entity<ModelParameter>());
        new ParameterConfig().Configure(builder.Entity<Parameter>());
    }
    //stuff
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<AssetDetail> AssetDetails => Set<AssetDetail>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Detail> Details => Set<Detail>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Model> Models => Set<Model>();
    public DbSet<ModelParameter> ModelParameters => Set<ModelParameter>();
    public DbSet<Parameter> Parameters => Set<Parameter>();
    public DbSet<Plant> Plants => Set<Plant>();
    // information
    public DbSet<CommunicateArea> CommunicateAreas => Set<CommunicateArea>();
    public DbSet<CommunicateAsset> CommunicateAssets => Set<CommunicateAsset>();
    public DbSet<Communicate> Communicates => Set<Communicate>();
    public DbSet<CommunicateCoordinate> CommunicateCoordinates => Set<CommunicateCoordinate>();
    public DbSet<CommunicateDevice> CommunicateDevices => Set<CommunicateDevice>();
    public DbSet<CommunicateModel> CommunicateModels => Set<CommunicateModel>();
    public DbSet<CommunicateSpace> CommunicateSpaces => Set<CommunicateSpace>();

    // location
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Coordinate> Coordinates => Set<Coordinate>();
    public DbSet<Space> Spaces => Set<Space>();

    // occurence
    public DbSet<SituationAsset> SituationAssets => Set<SituationAsset>();
    public DbSet<SituationCategory> SituationCategories => Set<SituationCategory>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Situation> Situations => Set<Situation>();
    public DbSet<SituationQuestion> SituationQuestions => Set<SituationQuestion>();
    public DbSet<SituationDetail> SituationDetails => Set<SituationDetail>();
    public DbSet<SituationParameter> SituationParameters => Set<SituationParameter>();


}
