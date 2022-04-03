namespace Sc3Hosted.Server.Entities;
public class SpaceCommunicate
{
    public int SpaceCommunicateId { get; set; }
    public int SpaceId { get; set; }
    public Space? Space { get; set; } 
    public int CommunicateId { get; set; }
    public Communicate? Communicate { get; set; } 
}
