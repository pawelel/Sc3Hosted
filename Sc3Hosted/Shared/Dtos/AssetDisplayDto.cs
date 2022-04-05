using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class AssetDisplayDto : BaseDto
{

    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public CoordinateFlat Coordinate { get; set; } = new();
    public int CoordinateId { get; set; }
    public SpaceFlat Space { get; set; } = new();
    public int SpaceId { get; set; }
    public AreaFlat Area { get; set; } = new();
    public int AreaId { get; set; }
    public PlantFlat Plant { get; set; } = new();
    public int PlantId { get; set; }
    public ModelFlat Model { get; set; } = new();
    public int ModelId { get; set; }
    public DeviceFlat Device { get; set; } = new();
    public int DeviceId { get; set; }
    public List<CategoryDto> Categories { get; set; } = new();
    public bool ShowDetails { get; set; }
}
