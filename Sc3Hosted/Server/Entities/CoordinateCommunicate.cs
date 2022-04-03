namespace Sc3Hosted.Server.Entities;
public class CoordinateCommunicate
{
    public int CoordinateCommunicateId { get; set; }
    public int CoordinateId { get; set; }
    public Coordinate? Coordinate { get; set; } 
    public int CommunicateId { get; set; }
    public Communicate? Communicate { get; set; } 
}
