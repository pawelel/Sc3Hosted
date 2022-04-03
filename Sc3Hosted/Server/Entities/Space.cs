using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Server.Entities;

public class Space 
{
    public int SpaceId { get; set; }
    public string Name { get; set; } = "";
    public virtual List<Coordinate>? Coordinates { get; set; } 
    public int AreaId { get; set; }
    public Area? Area { get; set; } 
    public SpaceType SpaceType { get; set; }
    public bool IsArchived { get; set; }
    public List<SpaceCommunicate>? SpaceCommunicates { get; set; } 
}
