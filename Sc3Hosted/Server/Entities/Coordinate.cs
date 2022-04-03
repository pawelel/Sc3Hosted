namespace Sc3Hosted.Server.Entities;

public class Coordinate  : BaseEntity
{
    public int CoordinateId { get; set; }
    public string Name { get; set; }=string.Empty;
    public string Description { get; set; }=string.Empty;
    public Space Space { get; set; } = new();
    public int SpaceId { get; set; }
    public bool IsArchived { get; set; }
    public virtual List<CoordinateCommunicate> CoordinateCommunicates { get; set; } = new();
}
