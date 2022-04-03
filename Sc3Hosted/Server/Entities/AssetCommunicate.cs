namespace Sc3Hosted.Server.Entities;
public class AssetCommunicate
{
    public int AssetCommunicateId { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; } 
    public int CommunicateId { get; set; }
    public Communicate? Communicate { get; set; }
}
