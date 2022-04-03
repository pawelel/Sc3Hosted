using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Server.Entities;

public class Space : BaseEntity
{
    public int SpaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<Coordinate> Coordinates { get; set; } = new();
    public int AreaId { get; set; }
    public Area Area { get; set; } = new();
    public SpaceType SpaceType { get; set; }
    
    public virtual List<SpaceCommunicate> SpaceCommunicates { get; set; } = new();
}
