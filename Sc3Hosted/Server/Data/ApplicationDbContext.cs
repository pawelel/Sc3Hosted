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
    //stuff
    public virtual DbSet<Asset> Assets => Set<Asset>();
    public virtual DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public virtual DbSet<AssetDetail> AssetDetails => Set<AssetDetail>();
    public virtual DbSet<Category> Categories => Set<Category>();
    public virtual DbSet<Detail> Details => Set<Detail>();
    public virtual DbSet<Device> Devices => Set<Device>();
    public virtual DbSet<Model> Models => Set<Model>();
    public virtual DbSet<ModelParameter> ModelParameters => Set<ModelParameter>();
    public virtual DbSet<Parameter> Parameters => Set<Parameter>();
    public virtual DbSet<Plant> Plants => Set<Plant>();
    // information
    public virtual DbSet<CommunicateArea> CommunicateAreas => Set<CommunicateArea>();
    public virtual DbSet<CommunicateAsset> CommunicateAssets => Set<CommunicateAsset>();
    public virtual DbSet<Communicate> Communicates => Set<Communicate>();
    public virtual DbSet<CommunicateCoordinate> CommunicateCoordinates => Set<CommunicateCoordinate>();
    public virtual DbSet<CommunicateDevice> CommunicateDevices => Set<CommunicateDevice>();
    public virtual DbSet<CommunicateModel> CommunicateModels => Set<CommunicateModel>();
    public virtual DbSet<CommunicateSpace> CommunicateSpaces => Set<CommunicateSpace>();
    public virtual DbSet<CommunicateCategory> CommunicateCategories => Set<CommunicateCategory>();

    // location
    public virtual DbSet<Area> Areas => Set<Area>();
    public virtual DbSet<Coordinate> Coordinates => Set<Coordinate>();
    public virtual DbSet<Space> Spaces => Set<Space>();

    // occurence
    public virtual DbSet<DeviceSituation> DeviceSituations => Set<DeviceSituation>();
    public virtual DbSet<CategorySituation> CategorySituations => Set<CategorySituation>();
    public virtual DbSet<Question> Questions => Set<Question>();
    public virtual DbSet<Situation> Situations => Set<Situation>();
    public virtual DbSet<SituationQuestion> SituationQuestions => Set<SituationQuestion>();
    public virtual DbSet<SituationDetail> SituationDetails => Set<SituationDetail>();
    public virtual DbSet<SituationParameter> SituationParameters => Set<SituationParameter>();
    public virtual DbSet<AssetSituation> AssetSituations => Set<AssetSituation>();

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
        new CommunicateCategoryConfig().Configure(builder.Entity<CommunicateCategory>());
        new AreaConfig().Configure(builder.Entity<Area>());
        new CoordinateConfig().Configure(builder.Entity<Coordinate>());
        new SpaceConfig().Configure(builder.Entity<Space>());
        new DeviceSituationConfig().Configure(builder.Entity<DeviceSituation>());
        new CategorySituationConfig().Configure(builder.Entity<CategorySituation>());
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
        new AssetSituationConfig().Configure(builder.Entity<AssetSituation>());
    }

}
