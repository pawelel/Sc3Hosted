using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;

public class AssetDto : BaseDto
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Status Status { get; set; }
    public virtual List<AssetDetailDto> AssetDetails { get; set; } = new();
    public virtual List<AssetCategoryDto> AssetCategories { get; set; } = new();
    public virtual List<CommunicateAssetDto> CommunicateAssets { get; set; } = new();
    public int CoordinateId { get; set; }
    public int ModelId { get; set; }
    public string Process { get; set; } = string.Empty;
    public  List<AssetSituationDto> AssetSituations { get; set; } = new();
}