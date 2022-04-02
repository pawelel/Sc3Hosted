using System.ComponentModel;

namespace Sc3Hosted.Shared.Dtos;

public class LocationDto
{
    [DisplayName("Zakład")]
    public string? Plant { get; set; }
    public int PlantId { get; set; }
    [DisplayName("Obszar")]
    public string? Area { get; set; }
    public int AreaId { get; set; }
    [DisplayName("Przestrzeń")]
    public string? Space { get; set; }
    public int SpaceId { get; set; }
    [DisplayName("Koordynat")]
    public string? Coordinate { get; set; }
    public int CoordinateId { get; set; }
}
