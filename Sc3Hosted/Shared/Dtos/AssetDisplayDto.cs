using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class AssetDisplayDto
{

    public int? AssetId { get; set; }
    public string? Name { get; set; }
    public string? Process { get; set; }
    public Status Status { get; set; }
    public CoordinateFlat? Coordinate { get; set; }
    public int? CoordinateId { get; set; }
    public SpaceFlat? Space { get; set; }
    public int? SpaceId { get; set; }
    public AreaFlat? Area { get; set; }
    public int? AreaId { get; set; }
    public PlantFlat? Plant { get; set; }
    public int? PlantId { get; set; }
    public ModelFlat? Model { get; set; }
    public int? ModelId { get; set; }
    public DeviceFlat? Device { get; set; }
    public int? DeviceId { get; set; }
    public List<CategoryDto> Categories { get; set; }
    public bool ShowDetails { get; set; }
}
