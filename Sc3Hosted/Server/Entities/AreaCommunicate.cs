namespace Sc3Hosted.Server.Entities;
public class AreaCommunicate
{
    public int AreaCommunicateId { get; set; }
    public int AreaId { get; set; }
    public Area? Area { get; set; } 
    public int CommunicateId { get; set; }
    public Communicate? Communicate { get; set; } 
}
