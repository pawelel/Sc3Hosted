using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;

public class SpaceDto : BaseDto
{
    public int SpaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }=string.Empty;
    public virtual List<CoordinateDto> Coordinates { get; set; } =new();
    public SpaceType SpaceType { get; set; }
    
    public virtual List<CommunicateSpaceDto> CommunicateSpaces { get; set; }=new();
}
