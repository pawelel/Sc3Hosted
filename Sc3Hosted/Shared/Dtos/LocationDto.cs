using System.ComponentModel;
namespace Sc3Hosted.Shared.Dtos;
public class LocationDto
{
    [DisplayName("Zakład")]
    public string Plant { get; set; } = string.Empty;
    public int PlantId { get; set; }
    [DisplayName("Obszar")]
    public string Area { get; set; } = string.Empty;
    public int AreaId { get; set; }
    [DisplayName("Przestrzeń")]
    public string Space { get; set; } = string.Empty;
    public int SpaceId { get; set; }
    [DisplayName("Koordynat")]
    public string Coordinate { get; set; } = string.Empty;
    public int CoordinateId { get; set; }
}
