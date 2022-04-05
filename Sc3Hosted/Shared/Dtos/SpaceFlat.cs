using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;

public class SpaceFlat : BaseDto
{

    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }=string.Empty;
    public int SpaceId { get; set; }
    public SpaceType SpaceType { get; set; }

}