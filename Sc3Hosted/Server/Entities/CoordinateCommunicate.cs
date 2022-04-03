namespace Sc3Hosted.Server.Entities;
public class CoordinateCommunicate : BaseEntity
{
    public int CoordinateCommunicateId { get; set; }
    public int CoordinateId { get; set; }
    public virtual Coordinate Coordinate { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
