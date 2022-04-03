namespace Sc3Hosted.Server.Entities;
public class AssetCommunicate : BaseEntity
{
    public int AssetCommunicateId { get; set; }
    public int AssetId { get; set; }
    public virtual Asset Asset { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
