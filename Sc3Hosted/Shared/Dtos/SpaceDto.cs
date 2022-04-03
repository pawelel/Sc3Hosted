using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;

public class SpaceDto
{
    public int SpaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }=string.Empty;
    public List<CoordinateDto> Coordinates { get; set; } =new();
    public SpaceType SpaceType { get; set; }
    
    public List<SpaceCommunicateDto> SpaceCommunicates { get; set; }=new();
}
