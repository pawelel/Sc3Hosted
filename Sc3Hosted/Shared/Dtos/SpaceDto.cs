using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;

public class SpaceDto
{
    public int SpaceId { get; set; }
    public string? Name { get; set; }
    public List<CoordinateDto> Coordinates { get; set; } 
    public SpaceType SpaceType { get; set; }
    public bool IsArchived { get; set; }
    public List<SpaceCommunicateDto> SpaceCommunicates { get; set; }
}
