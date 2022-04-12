namespace Sc3Hosted.Server.Entities;
public class CommunicateCoordinate : BaseEntity
{

    public int CoordinateId { get; set; }
    public virtual Coordinate Coordinate { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
