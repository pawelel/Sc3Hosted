using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class AssetDisplayDto : BaseDto
{

    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public string CoordinateName { get; set; } = string.Empty;
    public int CoordinateId { get; set; }
    public string SpaceName { get; set; } = string.Empty;
    public int SpaceId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string PlantName { get; set; } = string.Empty;
    public int PlantId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public virtual List<AssetDetailDisplayDto> Details { get; init; } = new();
    public int DeviceId { get; set; }
    public virtual List<AssetCategoryDisplayDto> Categories { get; init; } = new();
    public bool ShowDetails { get; set; }
    public virtual List<ModelParameterDisplayDto> Parameters { get; init; } = new();
}
